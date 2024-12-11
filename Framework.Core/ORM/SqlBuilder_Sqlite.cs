using Dapper;
using Framework.Core.Abstractions;
using System.Data;
using System.Reflection;
using System.Text;
using static Framework.Core.ORM.SqlBuilderFactory;

namespace Framework.Core.ORM
{
    public class SqlBuilder_Sqlite<T>:SqlBuilder_Base<T>, ISqlBuilder<T>, ISqlBuilderPartition
        where T : class
    {
        protected override char GetNamePrefix()
        {
            return '[';
        }

        protected override char GetNameSuffix()
        {
            return ']';
        }

        protected override string BuildInsertedIdSql()
        {
            return "(SELECT last_insert_rowid())";
        }

        public override string GetSelectTopRecordsSql(int number)
        {
            var selectStatement = $"Select Top {number} {GetFieldList()} FROM {TableNameForSql()} ; ";
            return selectStatement;
        }

        public override string GetQueryTopRecordsSql(int number,string where)
        {
            var selectStatement = $"Select Top {number} {GetFieldList()} FROM {TableNameForSql()} {where} ; ";
            return selectStatement;
        }

        public SqlBuilder_Sqlite(IDataTablePrefixProvider dataTablePrefixProvider = null,
            IUpdateCommandGroupProvider updateCmdGroupProvider = null)
        {
            Analyze<T>(dataTablePrefixProvider, updateCmdGroupProvider);
        }

        protected override  string GetSelectByPagerSql(string filter, string orderby, int pageIndex, int pageSize)
        {
            int skip = 1;
            if (pageIndex > 0)
            {
                skip = (pageIndex - 1) * pageSize + 1;
            }
            StringBuilder sb = new StringBuilder();
            var fieldList = GetFieldList();

            sb.AppendFormat(@$"SELECT  {fieldList} FROM(
                               SELECT ROW_NUMBER() OVER({orderby}) AS RowNum,{fieldList} 
                                FROM  {TableNameForSql()} {filter}) AS result
                                WHERE  RowNum >= {skip}   AND RowNum <= {pageIndex * pageSize}
                                {orderby}");
            return sb.ToString();
        }

        public string GetSelectByFilterSql(string filter, int recordNumber)
        {
            return $"Select Top {recordNumber} { GetFieldList() } FROM {TableNameForSql() } WHERE {filter}; ";
        }

        public string GetSelectByFilterSql(string filter, int recordNumber, string orderBy)
        {
            return $"Select Top {recordNumber} { GetFieldList() } FROM {TableNameForSql() } WHERE {filter} Order by {orderBy}; ";
        }

        public string GetSearchSqlByFilterOptions(FilterOptions opts)
        {
            string orderby = "";
            if (opts.OrderByStatements != null && opts.OrderByStatements.Count > 0)
                orderby = $" order by {string.Join(',', opts.OrderByStatements)}";
            string where = "";
            if (string.IsNullOrEmpty(opts.WhereClause) == false && opts.WhereClause.Trim().Length > 0)
                where = $" WHERE {opts.WhereClause}";

            if (opts.Pager != null)
            {
                return GetSelectByPagerSql(where, orderby, opts.Pager.PageIndex, opts.Pager.PageSize);
            }
            var top = "";
            if (opts.TakeNumber != null)
                top = $" TOP {opts.TakeNumber.Value} ";

            return $"SELECT {top} { GetFieldList() } FROM {TableNameForSql()} {where} {orderby};";
        }

        public override string GetSelectTopRecordsSql(int number, string group)
        {
            var selectStatement = $"Select Top {number} {GetFieldList(group)} FROM {TableNameForSql()} ; ";
            return selectStatement;
        }

        public override string GetSelectByFilterSql(string filter, int recordNumber, string orderBy, string group)
        {
            return $"Select Top {recordNumber} {GetFieldList(group)} FROM {TableNameForSql()} WHERE {filter} Order by {orderBy}; ";
        }

        protected override DBTypes GetDbType()
        {
            return DBTypes.SQLITE;
        }

        protected override string BuildBatchInsertValuePart(List<string> values)
        {
            return string.Join(" UNION ", values.Select(x => " SELECT " + x));
        }

    }
}
