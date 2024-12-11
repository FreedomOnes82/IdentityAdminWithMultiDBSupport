using Dapper;
using Framework.Core.Abstractions;
using Framework.Core.LambdaToSQL.Extensions;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using static Framework.Core.ORM.SqlBuilderFactory;

namespace Framework.Core.ORM
{

    public class SqlBuilder_MySql<T> : SqlBuilder_Base<T>, ISqlBuilder<T>, ISqlBuilderPartition
        where T : class
    {

        protected override char GetNamePrefix()
        {
            return '`';
        }

        protected override char GetNameSuffix()
        {
            return '`';
        }


        protected override string BuildInsertedIdSql()
        {
            return "(select @@IDENTITY)";
        }

        public override string GetSelectTopRecordsSql(int number)
        {
            var selectStatement = $"Select {GetFieldList()} FROM {TableNameForSql()} limit {number}; ";
            return selectStatement;
        }

        public override string GetQueryTopRecordsSql(int number, string where)
        {
            var selectStatement = $"Select {GetFieldList()} FROM {TableNameForSql()} {where} limit {number}; ";
            return selectStatement;
        }

        public SqlBuilder_MySql(IDataTablePrefixProvider dataTablePrefixProvider = null,
            IUpdateCommandGroupProvider updateCmdGroupProvider = null)
        {
            Analyze<T>(dataTablePrefixProvider, updateCmdGroupProvider);
        }

        protected override string GetSelectByPagerSql(string filter, string orderby, int pageIndex, int pageSize)
        {
            int skip = 0;
            StringBuilder sb = new StringBuilder();
            var fieldList = GetFieldList();

            if (pageIndex > 1)
            {
                skip = (pageIndex - 1) * pageSize + 1;
                sb.AppendFormat(@$"SELECT  {fieldList} FROM {TableNameForSql()} {filter} {orderby} limit {skip},{pageSize}");
            }
            else
                sb.AppendFormat(@$"SELECT  {fieldList} FROM {TableNameForSql()} {filter} {orderby} limit {pageSize}");

            return sb.ToString();
        }


        public string GetSelectByFilterSql(string filter, int recordNumber)
        {
            return $"Select {GetFieldList()} FROM {TableNameForSql()} WHERE {filter} limit {recordNumber}; ";
        }

        public string GetSelectByFilterSql(string filter, int recordNumber, string orderBy)
        {
            return $"Select {GetFieldList()} FROM {TableNameForSql()} WHERE {filter} Order by {orderBy} limit {recordNumber}; ";
        }

        public string GetSearchSqlByFilterOptions(FilterOptions opts)
        {
            string orderby = "";
            if (opts.OrderByStatements != null && opts.OrderByStatements.Count > 0)
                orderby = $" order by {string.Join(',', opts.OrderByStatements)}";
            string where = "";
            if (string.IsNullOrEmpty(opts.WhereClause) == false && opts.WhereClause.Trim().Length > 0)
                where = $" WHERE {opts.WhereClause}";

            if (opts.Pager == null && opts.TakeNumber != null)
            {
                opts.Pager = new PagerInfo() { PageSize = opts.TakeNumber.Value, PageIndex = 1 };
                //top = $" TOP {opts.TakeNumber.Value} ";
            }

            if (opts.Pager != null)
            {
                //Pager index start from 1
                return GetSelectByPagerSql(where, orderby, opts.Pager.PageIndex, opts.Pager.PageSize);
            }

            return $"SELECT {GetFieldList()} FROM {TableNameForSql()} {where} {orderby};";
        }


        public override string GetSelectTopRecordsSql(int number, string group)
        {
            var selectStatement = $"Select {GetFieldList(group)} FROM {TableNameForSql()} limit {number}; ";
            return selectStatement;
        }

        public override string GetSelectByFilterSql(string filter, int recordNumber, string orderBy, string group)
        {
            return $"Select {GetFieldList(group)} FROM {TableNameForSql()} WHERE {filter} Order by {orderBy} limit {recordNumber}; ";
        }

        protected override DBTypes GetDbType()
        {
            return DBTypes.MYSQL;
        }


    }
}
