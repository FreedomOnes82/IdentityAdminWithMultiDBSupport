using Dapper;
using Framework.Core.Abstractions;
using Framework.Core.LambdaToSQL.Extensions;
using Framework.Core.ORM.LambdaToSQL;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using static Framework.Core.ORM.SqlBuilderFactory;

namespace Framework.Core.ORM
{
    public abstract class SqlBuilder_Base<T> where T : class
    {
       
        protected string _typeName = "";
        protected TableMapping _tbMapping = null;
        protected string _tablePrefix = string.Empty;
        protected IUpdateCommandGroupProvider _updateCmdGroupProvider;

        protected virtual void Analyze<T>(IDataTablePrefixProvider dataTablePrefixProvider = null,
            IUpdateCommandGroupProvider updateCmdGroupProvider = null)
        {
            Type type = typeof(T);
            _typeName = type.FullName;
            bool existing = false;
            lock (DBModelAnalysisContext.Mappings)
            {
                existing = DBModelAnalysisContext.Mappings.ContainsKey(_typeName);
            }
            if (!existing)
            {
                TableMapping tbMapping = new TableMapping();
                var tableAttr = type.GetCustomAttribute<DataTableAttribute>();
                tbMapping.TableName = tableAttr == null ? type.Name : tableAttr.TableName;

                var properties = type.GetProperties();
                foreach (var prop in properties)
                {
                    if (prop.CustomAttributes.Any(x => x.AttributeType == typeof(DBFieldNotMappedAttribute)) ||
                         prop.PropertyType.Name.StartsWith("List"))
                    {
                        continue;
                    }
                    var propMapping = new PropMapping() { PropertyName = prop.Name, PropertyType = prop.PropertyType };
                    var fieldInfo = prop.GetCustomAttribute<DBFieldAttribute>(true);
                    if (fieldInfo != null)
                    {
                        propMapping.IsKey = fieldInfo.IsKeyField;
                        propMapping.FieldName = fieldInfo.FieldName;
                        propMapping.AutoGenerate = fieldInfo.AutoGenerate;
                        if (fieldInfo.Groups != null)
                        {
                            propMapping.FieldGroups = fieldInfo.Groups.ToList();
                        }
                    }
                    tbMapping.PropMappings.Add(propMapping);
                }
                lock (DBModelAnalysisContext.Mappings)
                {
                    DBModelAnalysisContext.Mappings.Add(type.FullName, tbMapping);
                }
            }
            lock (DBModelAnalysisContext.Mappings)
                _tbMapping = DBModelAnalysisContext.Mappings[type.FullName];

            if (dataTablePrefixProvider != null)
            {
                _tablePrefix = dataTablePrefixProvider.Prefix;
            }
            _updateCmdGroupProvider = updateCmdGroupProvider;
        }

        protected abstract char GetNamePrefix();
        protected abstract char GetNameSuffix();
        protected virtual string PopulateName(string rawName)
        {
            return GetNamePrefix() + rawName + GetNameSuffix();
        }
        protected abstract string GetSelectByPagerSql(string filter, string orderby, int pageIndex, int pageSize);
        public abstract string GetSelectTopRecordsSql(int number);
        public abstract string GetQueryTopRecordsSql(int number, string where);

        protected virtual string TableNameForSql()
        {
            return PopulateName(_tablePrefix + _tbMapping.TableName);
        }

        protected virtual string TableNameForSql<Model>(Model m) where Model : PartitionModelBase
        {
            return PopulateName(_tablePrefix + _tbMapping.TableName + m.GetPartition() );
        }

        public virtual string GetInsertSql()
        {

                string[] strSqlNames = _tbMapping.PropMappings.Where(x => x.AutoGenerate == false).Select(p => $"{PopulateName( p.FieldName ) }").ToArray();
                string strSqlName = string.Join(",", strSqlNames);
                string[] strSqlValues = _tbMapping.PropMappings.Where(x => x.AutoGenerate == false).Select(P => $"{P.ParameterName}").ToArray();
                string strSqlValue = string.Join(",", strSqlValues);
                string insertSql = $"insert into {TableNameForSql()} ( {strSqlName} ) values ({strSqlValue});";

                return insertSql;
        }


        public virtual string GetDeleteSqlByWhereClause(string filter)
        {
            return $"DELETE FROM {TableNameForSql()} WHERE {filter}";
        }

        protected virtual string GetFieldList(string group = "")
        {
            IEnumerable<PropMapping> propMappings = _tbMapping.PropMappings;
            if (group != "")
            {
                var fieldsOfGroup = FilterFieldByGroup(group);
                propMappings = propMappings.Where(x => fieldsOfGroup.Contains(x.FieldName));
            }
            var fieldListStr = string.Join(",", propMappings.Select(x => x.FieldName == x.PropertyName ?
                $"{PopulateName(x.PropertyName)}" : $"{PopulateName(x.FieldName)} as {PopulateName(x.PropertyName)}").ToArray());
            return fieldListStr;
        }

        protected List<Func<T, object>> FilterGetters(List<PropMapping> props)
        {
            var getters = PreCompileGetterHelper<T>.Getters;
            List<Func<T, object>> rs = new List<Func<T, object>>();
            props.ForEach(y => {
                rs.Add(getters.First(x => x.Key == y.PropertyName).Value);
            });
            return rs;
        }

        protected string BuildSelectStatement(string group = "")
        {
            return $"Select {GetFieldList()} FROM {TableNameForSql()} ";
        }

        public string GetSelectAllSql()
        {
            var selectStatement = $"{BuildSelectStatement()} ; ";
            return selectStatement;
        }

        public virtual string GetCountSql(string filter)
        {
            StringBuilder sb = new StringBuilder();
            string where = "";
            if (string.IsNullOrEmpty(filter) == false && filter.Trim().Length > 0)
                where = $" WHERE {filter}";
            sb.AppendFormat($"SELECT COUNT(1) FROM {TableNameForSql()} {where};");
            return sb.ToString();
        }

        public virtual string GetSelectByFilterSql(string filter)
        {
            return $"Select {GetFieldList()} FROM {TableNameForSql()} WHERE {filter}; ";
        }

        protected string GetKeyFilter()
        {
            var filterStatements = _tbMapping.PropMappings.Where(x => x.IsKey == true).Select(x => $" {PopulateName(x.FieldName)}={x.ParameterName}");
            return $" WHERE {string.Join(" AND ", filterStatements)}";
        }

        public  string GetSelectByKeySql()
        {
            var fieldListStr = string.Join(",", _tbMapping.PropMappings.Select(x => x.FieldName == x.PropertyName ? 
            $"{PopulateName(x.PropertyName)}" : $"{PopulateName(x.FieldName)} as {PopulateName(x.PropertyName)}").ToArray());
            var selectStatement = $"Select {fieldListStr} FROM {TableNameForSql()} {GetKeyFilter()}; ";

            return selectStatement;
        }
        public  string GetUpdateSql()
        {
            var fieldListStr = string.Join(",", _tbMapping.PropMappings.Where(x => x.AutoGenerate == false && x.IsKey == false).
                Select(x => $"{PopulateName(x.FieldName)} = @{x.PropertyName}").ToArray());
            var updateStatement = $"UPDATE {TableNameForSql()} SET {fieldListStr} {GetKeyFilter()} ;";
            return updateStatement;
        }

        public  string GetUpdateAndReturnSql()
        {
            return GetUpdateSql() + GetSelectByKeySql();
        }


        public virtual string GetDeleteByKey()
        {
            return $"DELETE FROM {TableNameForSql()} {GetKeyFilter()}";
        }

        protected virtual List<string> FilterFieldByGroup(string group)
        {
            List<string> fieldsOfGroup = new List<string>();
          
            fieldsOfGroup = _tbMapping.PropMappings.Where(x => x.FieldGroups.Contains(group)).Select(x => x.FieldName).ToList();
            return fieldsOfGroup;
        }

        #region "Group Op"
        public virtual string GetUpdateSql(string group)
        {
            var fieldsOfGroup = FilterFieldByGroup(group);

            var fieldListStr = string.Join(",", _tbMapping.PropMappings.Where(x =>
            x.AutoGenerate == false && x.IsKey == false && fieldsOfGroup.Contains(x.FieldName)).
                Select(x => $"{PopulateName(x.FieldName)} = @{x.PropertyName}").ToArray());
            var updateStatement = $"UPDATE {TableNameForSql()} SET {fieldListStr} {GetKeyFilter()} ;";
            return updateStatement;
        }

        public virtual string GetUpdateAndReturnSql(string group)
        {
            return GetUpdateSql(group) + GetSelectByKeySql();
        }


        public virtual string GetInsertSql(string group)
        {
            var fieldsOfGroup = FilterFieldByGroup(group);
            string[] strSqlNames = _tbMapping.PropMappings.Where(x => x.AutoGenerate == false 
            && fieldsOfGroup.Contains(x.FieldName)).Select(p => $"{PopulateName(p.FieldName)}").ToArray();
            string strSqlName = string.Join(",", strSqlNames);
            string[] strSqlValues = _tbMapping.PropMappings.Where(x => x.AutoGenerate == false).Select(P => $"{P.ParameterName}").ToArray();
            string strSqlValue = string.Join(",", strSqlValues);
            string insertSql = $"insert into {TableNameForSql()} ( {strSqlName} ) values ({strSqlValue});";

            return insertSql;
        }

        public virtual string GetInsertAndReturnSql(string group)
        {
            var insertSql = GetInsertSql(group);

            string returnFilter = "";
            var autoGeneratedMapping = _tbMapping.PropMappings.FirstOrDefault(x => x.AutoGenerate == true);
            if (autoGeneratedMapping != null)
            {
                returnFilter = $" WHERE {autoGeneratedMapping.FieldName}=  {BuildInsertedIdSql()}";
            }
            else
            {
                //var filterStatements = _tbMapping.PropMappings.Where(x => x.IsKey == true).Select(x => $" {x.FieldName}={x.ParameterName}");
                returnFilter = GetKeyFilter();// $" WHERE  { string.Join(" and ", filterStatements)}";
            }
            //var fieldListStr = string.Join(",", _tbMapping.PropMappings.Select(x => x.FieldName == x.PropertyName ? $"{PopulateName(x.PropertyName)}" : $"{PopulateName(x.FieldName)} as {PopulateName(x.PropertyName)}").ToArray());
            //var selectStatement = $"Select {GetFieldListStr()} FROM {TableNameForSql()} ";

            return insertSql + BuildSelectStatement() + returnFilter;
        }
        #endregion


        protected abstract string BuildInsertedIdSql();
        //(Select Cast(SCOPE_IDENTITY() as INT)) for mssql and (select @@IDENTITY) for mysql

        public virtual string GetInsertAndReturnSql()
        {
            var insertSql = GetInsertSql();

            string returnFilter = "";
            var autoGeneratedMapping = _tbMapping.PropMappings.FirstOrDefault(x => x.AutoGenerate == true);
            if (autoGeneratedMapping != null)
            {
                returnFilter = $" WHERE {autoGeneratedMapping.FieldName}=  {BuildInsertedIdSql()}";
            }
            else
            {
                //var filterStatements = _tbMapping.PropMappings.Where(x => x.IsKey == true).Select(x => $" {x.FieldName}={x.ParameterName}");
                returnFilter = GetKeyFilter();// $" WHERE  { string.Join(" and ", filterStatements)}";
            }
            //var fieldListStr = string.Join(",", _tbMapping.PropMappings.Select(x => x.FieldName == x.PropertyName ? $"{PopulateName(x.PropertyName)}" : $"{PopulateName(x.FieldName)} as {PopulateName(x.PropertyName)}").ToArray());
            //var selectStatement = $"Select {GetFieldListStr()} FROM {TableNameForSql()} ";

            return insertSql + BuildSelectStatement() + returnFilter;
        }

       

        #region "Partition operation"

        public virtual string GetInsertSql<T1>(T1 model) where T1 : PartitionModelBase
        {
            string[] strSqlNames = _tbMapping.PropMappings.Where(x => x.AutoGenerate == false).Select(p => $"{PopulateName(p.FieldName)}").ToArray();
            string strSqlName = string.Join(",", strSqlNames);
            string[] strSqlValues = _tbMapping.PropMappings.Where(x => x.AutoGenerate == false).Select(P => $"{P.ParameterName}").ToArray();
            string strSqlValue = string.Join(",", strSqlValues);
            string insertSql = $"insert into {TableNameForSql<T1>(model)} ( {strSqlName} ) values ({strSqlValue});";

            return insertSql;
        }

        public string GetInsertAndReturnSql<T1>(T1 model) where T1 : PartitionModelBase
        {
            var insertSql = GetInsertSql<T1>(model);

            string returnFilter = "";
            var autoGeneratedMapping = _tbMapping.PropMappings.FirstOrDefault(x => x.AutoGenerate == true);
            if (autoGeneratedMapping != null)
            {
                returnFilter = $" WHERE {autoGeneratedMapping.FieldName}= {BuildInsertedIdSql()}";
            }
            else
            {
                //var filterStatements = _tbMapping.PropMappings.Where(x => x.IsKey == true).Select(x => $" {x.FieldName}={x.ParameterName}");
                returnFilter = GetKeyFilter();// $" WHERE  { string.Join(" and ", filterStatements)}";
            }
            var fieldListStr = string.Join(",", _tbMapping.PropMappings.Select(x => x.FieldName == x.PropertyName ? $"{PopulateName(x.PropertyName)}" : $"{PopulateName(x.FieldName)} as {PopulateName(x.PropertyName)}").ToArray());
            var selectStatement = $"Select {fieldListStr} FROM {TableNameForSql<T1>(model)} ";

            return insertSql + selectStatement + returnFilter;
        }

        public virtual string GetInsertSql<T1>(string group,T1 model) where T1 : PartitionModelBase
        {
            var fieldsOfGroup = FilterFieldByGroup(group);
            string[] strSqlNames = _tbMapping.PropMappings.Where(x => x.AutoGenerate == false && fieldsOfGroup.Contains(x.FieldName))
                .Select(p => $"{PopulateName(p.FieldName)}").ToArray();
            string strSqlName = string.Join(",", strSqlNames);
            string[] strSqlValues = _tbMapping.PropMappings.Where(x => x.AutoGenerate == false && fieldsOfGroup.Contains(x.FieldName))
                .Select(P => $"{P.ParameterName}").ToArray();
            string strSqlValue = string.Join(",", strSqlValues);
            string insertSql = $"insert into {TableNameForSql<T1>(model)} ( {strSqlName} ) values ({strSqlValue});";

            return insertSql;
        }

        public string GetInsertAndReturnSql<T1>(string group, T1 model) where T1 : PartitionModelBase
        {
            var insertSql = GetInsertSql<T1>(group,model);

            string returnFilter = "";
            var autoGeneratedMapping = _tbMapping.PropMappings.FirstOrDefault(x => x.AutoGenerate == true);
            if (autoGeneratedMapping != null)
            {
                returnFilter = $" WHERE {autoGeneratedMapping.FieldName}= {BuildInsertedIdSql()}";
            }
            else
            {
                //var filterStatements = _tbMapping.PropMappings.Where(x => x.IsKey == true).Select(x => $" {x.FieldName}={x.ParameterName}");
                returnFilter = GetKeyFilter();// $" WHERE  { string.Join(" and ", filterStatements)}";
            }
            var fieldListStr = string.Join(",", _tbMapping.PropMappings.Select(x => x.FieldName == x.PropertyName ?
            $"{PopulateName(x.PropertyName)}" : $"{PopulateName(x.FieldName)} as {PopulateName(x.PropertyName)}").ToArray());
            var selectStatement = $"Select {fieldListStr} FROM {TableNameForSql<T1>(model)} ";

            return insertSql + selectStatement + returnFilter;
        }

        public virtual string GetSelectByKeySql<T1>(T1 model) where T1 : PartitionModelBase
        {
            var fieldListStr = string.Join(",", _tbMapping.PropMappings.Select(x => x.FieldName == x.PropertyName ?
            $"{PopulateName(x.PropertyName)}" : $"{PopulateName(x.FieldName)} as {PopulateName(x.PropertyName)}").ToArray());
            var selectStatement = $"Select {fieldListStr} FROM {TableNameForSql<T1>(model)} {GetKeyFilter()}; ";

            return selectStatement;
        }

        public virtual string GetSelectByKeySql<T1>(T1 model,string group) where T1 : PartitionModelBase
        {
            var fieldListStr = string.Join(",", _tbMapping.PropMappings.Select(x => x.FieldName == x.PropertyName ?
            $"{PopulateName(x.PropertyName)}" : $"{PopulateName(x.FieldName)} as {PopulateName(x.PropertyName)}").ToArray());
            var selectStatement = $"Select {fieldListStr} FROM {TableNameForSql<T1>(model)} {GetKeyFilter()}; ";

            return selectStatement;
        }

        public virtual string GetUpdateSql<T1>(T1 model) where T1 : PartitionModelBase
        {
            var autoGeneratedMapping = _tbMapping.PropMappings.FirstOrDefault(x => x.AutoGenerate == true);

            var fieldListStr = string.Join(",", _tbMapping.PropMappings.Where(x => x.AutoGenerate == false && x.IsKey == false).
                Select(x => $"{PopulateName(x.FieldName)} = @{x.PropertyName}").ToArray());
            var updateStatement = $"UPDATE {TableNameForSql<T1>(model)} SET {fieldListStr} {GetKeyFilter()} ;";
            return updateStatement;
        }

        public virtual string GetUpdateAndReturnSql<T1>(T1 model) where T1 : PartitionModelBase
        {
            return GetUpdateSql<T1>(model) + GetSelectByKeySql<T1>(model);
        }

        public virtual string GetDeleteByKey<T1>(T1 model) where T1 : PartitionModelBase
        {
            return $"DELETE FROM {TableNameForSql<T1>(model)} {GetKeyFilter()}";
        }

        public virtual string GetUpdateSql<T1>(string group, T1 model) where T1 : PartitionModelBase
        {
            var fieldsOfGroup = FilterFieldByGroup(group);

            var fieldListStr = string.Join(",", _tbMapping.PropMappings.Where(x =>
            x.AutoGenerate == false && x.IsKey == false && fieldsOfGroup.Contains(x.FieldName)).
                Select(x => $"{PopulateName(x.FieldName)} = @{x.PropertyName}").ToArray());
            var updateStatement = $"UPDATE {TableNameForSql<T1>(model)} SET {fieldListStr} {GetKeyFilter()} ;";
            return updateStatement;
        }

        public virtual string GetUpdateAndReturnSql<T1>(string group, T1 model) where T1 : PartitionModelBase
        {
            return GetUpdateSql<T1>(group, model) + GetSelectByKeySql<T1>(model);
        }


        #endregion

        #region "Select by group"
        public abstract string GetSelectTopRecordsSql(int number, string group);

        public virtual string GetSelectAllSql(string group)
        {
            var selectStatement = $"Select {GetFieldList(group)} FROM {TableNameForSql()} ; ";
            return selectStatement;
        }

        public virtual string GetSelectByFilterSql(string filter, string group)
        {
            return $"Select {GetFieldList(group)} FROM {TableNameForSql()} WHERE {filter}; ";
        }

        public abstract string GetSelectByFilterSql(string filter, int recordNumber, string orderBy, string group);

        public virtual string GetSelectByKeySql(string group)
        {
            var selectStatement = $"Select {GetFieldList(group)} FROM {TableNameForSql()} {GetKeyFilter()}; ";

            return selectStatement;
        }
        #endregion

        #region "Create"

        public virtual string GetCreateSql()
        {
            var typeDecs = _tbMapping.PropMappings.Select(x =>
            {
                var typeDeclaration = "";
                var type = x.PropertyType;
                var nullable = " ";
                if (x.PropertyType.IsGenericType && x.PropertyType.Name == "Nullable`1")
                {
                    type = x.PropertyType.GetGenericArguments()[0];
                    nullable = " NULL ";
                }
                if (type == typeof(System.String))
                {
                    typeDeclaration = " Nvarchar(Max)";
                }

                if (type == typeof(System.Int32))
                {
                    typeDeclaration = " INT";
                }

                if (type == typeof(System.Byte))
                {
                    typeDeclaration = "TINYINT";
                }

                if (type == typeof(System.Int16))
                {
                    typeDeclaration = " SMALLINT";
                }

                if (type == typeof(System.Int64))
                {
                    typeDeclaration = " BIGINT";
                }

                if (type == typeof(System.Boolean))
                {
                    typeDeclaration = " BIT";
                }

                if (type == typeof(System.DateTime))
                {
                    typeDeclaration = " DateTime";
                }
                if (type == typeof(System.Decimal))
                {
                    typeDeclaration = " Decimal(18,8)";
                }
                string rs = $"[{x.FieldName}] {typeDeclaration}";

                if (x.AutoGenerate)
                {
                    rs += " IDENTITY(1,1) ";
                }

                return rs + nullable;
            });

            var keyFieldNames = _tbMapping.PropMappings.Where(x => x.IsKey).Select(x => x.FieldName);
            typeDecs = typeDecs.Append("   Primary Key(" + string.Join(",", keyFieldNames) + ")");

            return $"CREATE TABLE {TableNameForSql()} ( {string.Join(",\r\n", typeDecs.ToArray())})";
        }
        #endregion

        public virtual WherePart GetWherePart(Expression<Func<T, bool>> expression)
        {
            return expression.ToSql<T>(TableNameForSql(), GetDbType());
        }

        protected virtual DBTypes GetDbType()
        {
            return DBTypes.SQLSERVER;
        }

        public virtual KeyValuePair<string, DynamicParameters> BuildQuery(Expression<Func<T, bool>> expression)
        {
            string selectStatement = BuildSelectStatement();
            var parameter = new DynamicParameters();
            var wherePart = GetWherePart(expression);
            string whereStatement = wherePart.HasSql ? $" WHERE {wherePart.Sql}" : string.Empty;
            foreach (var param in wherePart.Parameters)
            {
                parameter.Add(param.Key, param.Value, param.Type);
            }
            string sql = $"{selectStatement} {whereStatement}"; // sql.Replace("{where}", whereSql.HasSql ? $" WHERE {whereSql.Sql}" : string.Empty);

            return new KeyValuePair<string, DynamicParameters>(sql, parameter);
        }

        public KeyValuePair<string, DynamicParameters> BuildQueryTop(Expression<Func<T, bool>> expression, int number)
        {
            var parameter = new DynamicParameters();
            var wherePart = GetWherePart(expression);
            string whereStatement = wherePart.HasSql ? $" WHERE {wherePart.Sql}" : string.Empty;
            string sql = GetQueryTopRecordsSql(number, whereStatement);
            foreach (var param in wherePart.Parameters)
            {
                parameter.Add(param.Key, param.Value, param.Type);
            }
            return new KeyValuePair<string, DynamicParameters>(sql, parameter);
        }

        public KeyValuePair<string, DynamicParameters> BuildQuery(FilterOptions<T> filterOpt)
        {

            var parameter = new DynamicParameters();
            var wherePart = GetWherePart(filterOpt.WhereClause);
            string whereStatement = wherePart.HasSql ? $" WHERE {wherePart.Sql}" : string.Empty;
            string sql = GetCountSql(wherePart.Sql) + GetSelectByPagerSql(whereStatement, string.Join(" ",filterOpt.OrderByStatements), filterOpt.Pager.PageIndex, filterOpt.Pager.PageSize);
            foreach (var param in wherePart.Parameters)
            {
                parameter.Add(param.Key, param.Value, param.Type);
            }
            return new KeyValuePair<string, DynamicParameters>(sql, parameter);
        }

        public virtual KeyValuePair<string, DynamicParameters> BuildDeleteSql(Expression<Func<T, bool>> expression)
        {
            var parameter = new DynamicParameters();
            var wherePart = GetWherePart(expression);
            string whereStatement = wherePart.HasSql ? $" WHERE {wherePart.Sql}" : string.Empty;
            var sql =  $"Delete from {TableNameForSql()} {whereStatement}";
            foreach (var param in wherePart.Parameters)
            {
                parameter.Add(param.Key, param.Value, param.Type);
            }
            return new KeyValuePair<string, DynamicParameters>(sql, parameter);
        }

        protected virtual string BuildBatchInsertValuePart(List<string> values)
        {
            return " values " + string.Join(",", values.Select(x => '(' + x + ')'));
        }

        public KeyValuePair<string, DynamicParameters> BuildBatchInsertSql(List<T> models)
        {
            var props = _tbMapping.PropMappings.Where(x => x.AutoGenerate == false).ToList();
            string[] strSqlNames = props.Select(p => $"{PopulateName(p.FieldName)}").ToArray();
            string strSqlName = string.Join(",", strSqlNames);

            int i = 0;
            List<string> valueStatements = new List<string>();
            var parameter = new DynamicParameters();
            List<Func<T, object>> getters = FilterGetters(props);
            foreach (var model in models)
            {
                List<string> propNames = new List<string>();

                for (int k = 0; k < getters.Count(); k++)
                {
                    var p = props[k];
                    string propName = p.PropertyName + i.ToString();
                    propNames.Add('@' + propName);
                    var value = getters[k](model);

                    parameter.Add(propName, value, ConvertToDbType(p.PropertyType));
                }

                valueStatements.Add(string.Join(',', propNames));

                i++;
            }

            var valueStr = BuildBatchInsertValuePart(valueStatements);
            string insertSql = $"insert into {TableNameForSql()} ( {strSqlName} ) {valueStr};";
            return new KeyValuePair<string, DynamicParameters>(insertSql, parameter);
        }

        protected static DbType ConvertToDbType(Type propertyType)
        {
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propertyType = Nullable.GetUnderlyingType(propertyType);
            }

            if (propertyType == typeof(int))
                return DbType.Int32;
            if (propertyType == typeof(string))
                return DbType.String;
            if (propertyType == typeof(DateTime))
                return DbType.DateTime;
            if (propertyType == typeof(bool))
                return DbType.Boolean;
            // Add more type mappings as needed
            if (propertyType == typeof(byte))
                return DbType.Byte;
            if (propertyType == typeof(sbyte))
                return DbType.SByte;
            if (propertyType == typeof(short))
                return DbType.Int16;

            if (propertyType == typeof(uint))
                return DbType.UInt32;
            if (propertyType == typeof(long))
                return DbType.Int64;
            if (propertyType == typeof(ulong))
                return DbType.UInt64;
            if (propertyType == typeof(decimal))
                return DbType.Decimal;
            if (propertyType == typeof(double))
                return DbType.Double;

            throw new NotSupportedException($"Type mapping not supported for {propertyType.Name}");
        }
    }
}

