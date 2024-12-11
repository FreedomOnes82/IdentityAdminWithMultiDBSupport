using Dapper;
using Framework.Core.LambdaToSQL.Extensions;
using System.Linq.Expressions;

namespace Framework.Core.Abstractions
{
    public interface ISqlBuilder<Model>
    {
        string GetInsertSql();
        string GetInsertSql(string group);

        string GetDeleteSqlByWhereClause(string filter);

        string GetInsertAndReturnSql();

        string GetInsertAndReturnSql(string group);

        string GetSelectTopRecordsSql(int number);

        string GetQueryTopRecordsSql(int number, string where);

        string GetSelectTopRecordsSql(int number,string group);

        string GetSelectAllSql();

        string GetSelectAllSql(string group);

        string GetCountSql(string filter);

        string GetSelectByFilterSql(string filter);

        string GetSelectByFilterSql(string filter, int recordNumber);

        string GetSelectByFilterSql(string filter, int recordNumber, string orderBy);

        string GetSelectByFilterSql(string filter,string group);

        string GetSelectByFilterSql(string filter, int recordNumber, string orderBy, string group);

        string GetSearchSqlByFilterOptions(FilterOptions opts);

        string GetSelectByKeySql();
        string GetSelectByKeySql(string group);

        string GetUpdateSql();

        string GetUpdateAndReturnSql();

        string GetUpdateSql(string group);

        string GetUpdateAndReturnSql(string group);

        string GetDeleteByKey();

        string GetCreateSql();

        //string GetBatchInsertSql();
        WherePart GetWherePart(Expression<Func<Model, bool>> expression);
        KeyValuePair<string, DynamicParameters> BuildQuery(Expression<Func<Model, bool>> expression);
        KeyValuePair<string, DynamicParameters> BuildQuery(FilterOptions<Model> filterOpt);
        KeyValuePair<string, DynamicParameters> BuildQueryTop(Expression<Func<Model, bool>> expression, int number);
        KeyValuePair<string, DynamicParameters> BuildDeleteSql(Expression<Func<Model, bool>> expression);

        KeyValuePair<string, DynamicParameters> BuildBatchInsertSql(List<Model> models);
    }
}
