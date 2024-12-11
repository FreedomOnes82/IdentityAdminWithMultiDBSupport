using Dapper;
using Framework.Core.Abstractions;
using Framework.Core.ORM;
using System.Data;
using System.Linq.Expressions;
using static Framework.Core.ORM.SqlBuilderFactory;

namespace Framework.Core.UOW
{
    public class Repository<Model> : BaseRepository<Model>, IRepository<Model> where Model : class
    {
        DBTypes _dbType = DBTypes.None;
        ISqlBuilder<Model> _sqlBuilder = null;

        public Repository(IDbConnection db,IDataTablePrefixProvider dataTablePrefixProvider, 
            IUpdateCommandGroupProvider updateCmdGroupProvider) : base(db)
        {
            InitSqlBuilder(DbConnection, dataTablePrefixProvider, updateCmdGroupProvider);
        }

        public Repository(IUnitOfWork uow, IDataTablePrefixProvider dataTablePrefixProvider,
            IUpdateCommandGroupProvider updateCmdGroupProvider) : base(uow)
        {
            InitSqlBuilder(DbConnection, dataTablePrefixProvider, updateCmdGroupProvider);
        }

        private void InitSqlBuilder(IDbConnection dbConn, IDataTablePrefixProvider dataTablePrefixProvider,
            IUpdateCommandGroupProvider updateCmdGroupProvider)
        {
            var connectionType = dbConn.GetType().FullName;
            if (_sqlBuilder == null)
                if (connectionType.Contains("Sqlite"))
                {
                    _sqlBuilder = SqlBuilderFactory.CreateInstance<Model>(DBTypes.SQLITE, 
                        dataTablePrefixProvider, updateCmdGroupProvider);
                }
                else
                {
                    if (connectionType.Contains("MySql", StringComparison.InvariantCultureIgnoreCase))
                        _sqlBuilder = SqlBuilderFactory.CreateInstance<Model>(DBTypes.MYSQL, 
                            dataTablePrefixProvider, updateCmdGroupProvider);
                    else
                        _sqlBuilder = SqlBuilderFactory.CreateInstance<Model>(DBTypes.SQLSERVER,
                            dataTablePrefixProvider, updateCmdGroupProvider);
                }
        }

        private ISqlBuilder<Model> CurrentSqlBuilder 
        {
            get
            {
                return _sqlBuilder;
            }
        }

        public async Task InsertAsync(Model data)
        {
            var insertSql = CurrentSqlBuilder.GetInsertSql();
            await ExecuteGetAffectAsync<Model>(insertSql, data);
        }

        public async Task InsertAsync(List<Model> models)
        {
            if (models == null || models.Count == 0)
                return;
            var insertSql = CurrentSqlBuilder.BuildBatchInsertSql(models);// GetInsertSql();
            await ExecuteGetAffectAsync<Model>(insertSql.Key,insertSql.Value);
        }

        public async Task<Model> InsertAndReturnAsync(Model data)
        {
            var insertSql = CurrentSqlBuilder.GetInsertAndReturnSql();
            return await InsertAndReturnAsync<Model>(insertSql, data);
        }

        public async Task<Model> UpdateAndReturnAsync(Model data)
        {
            var updateAndReturnSql = CurrentSqlBuilder.GetUpdateAndReturnSql();
            return await UpdateAndReturnAsync<Model>(updateAndReturnSql, data);
        }


        public async Task<int> UpdateAsync(List<Model> models)
        {
            var updateSql = CurrentSqlBuilder.GetUpdateSql();
            return await ExecuteGetAffectAsync<Model>(updateSql, models);
        }

        public async Task UpdateAsync(Model data)
        {
            var updateSql = CurrentSqlBuilder.GetUpdateSql();
            await ExecuteAsync<Model>(updateSql, data);
        }

        public async Task DeleteAsync(Model data)
        {
            var updateSql = CurrentSqlBuilder.GetDeleteByKey();
            await ExecuteGetAffectAsync<Model>(updateSql, data);
        }

        /// <summary>
        /// Delete multiple records by keys
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public async Task DeleteAsync(List<Model> models)
        {
            var updateSql = CurrentSqlBuilder.GetDeleteByKey();
            await ExecuteGetAffectAsync<Model>(updateSql, models);
        }

        public async Task DeleteAsync(Expression<Func<Model, bool>> exp)
        {
            var sql = CurrentSqlBuilder.BuildDeleteSql(exp);
            await ExecuteGetAffectAsync<Model>(sql.Key, sql.Value);
        }

        public async Task<Model> GetSingleOrDefaultByKeyAsync(Model data)
        {
            var getByKeyCmd = CurrentSqlBuilder.GetSelectByKeySql();
            return await QuerySingleOrDefaultAsync(getByKeyCmd, data);
        }
        public async Task<Model> GetByKeyAsync(Model data)
        {
            var getByKeyCmd = CurrentSqlBuilder.GetSelectByKeySql();
            return await QuerySingleAsync(getByKeyCmd, data);
        }

        public async Task<IEnumerable<Model>> GetAllAsync()
        {
            var query = CurrentSqlBuilder.GetSelectAllSql();
            return await QueryAsync<Model>(query, null);
        }

        public async Task<int> GetCountAsync(string whereCmd, object parameterObject)
        {
            var countSql = CurrentSqlBuilder.GetCountSql(whereCmd);
            var rs = await QuerySingleOrDefaultAsync<object,int>(countSql, parameterObject);
            return rs;
        }

        public async Task<IEnumerable<Model>> GetTopRecordsAsync(int number)
        {
            var getByKeyCmd = CurrentSqlBuilder.GetSelectTopRecordsSql(number);
            return await QueryAsync<Model>(getByKeyCmd, null);
        }

        public async Task<IEnumerable<Model>> SearchAsync(string whereCmd, object parameterObject)
        {
            var getAllCmd = CurrentSqlBuilder.GetSelectByFilterSql(whereCmd);
            return await QueryAsync<Model>(getAllCmd, parameterObject);
        }

        public async Task<IEnumerable<Model>> SearchAsync(string whereCmd, object parameterObject, int recordNumber)
        {
            var getAllCmd = CurrentSqlBuilder.GetSelectByFilterSql(whereCmd, recordNumber);
            return await QueryAsync<Model>(getAllCmd, parameterObject);
        }

        public async Task<IEnumerable<Model>> SearchAsync(string whereCmd, object parameterObject, int recordNumber, string orderBy)
        {
            var getAllCmd = CurrentSqlBuilder.GetSelectByFilterSql(whereCmd, recordNumber, orderBy);
            return await QueryAsync<Model>(getAllCmd, parameterObject);
        }

        public async Task<IEnumerable<Model>> SearchByFilterOpts(FilterOptions opts, object parameterObject)
        {
            var sql = CurrentSqlBuilder.GetSearchSqlByFilterOptions(opts);
            return await QueryAsync<Model>(sql, parameterObject);
        }

        public async Task<PagerResult<Model>> SearchAndCountByFilterOpts(FilterOptions opts, object parameterObject)
        {
            var countSql = CurrentSqlBuilder.GetCountSql(opts.WhereClause);
            var pagerSql = CurrentSqlBuilder.GetSearchSqlByFilterOptions(opts);
            PagerResult<Model> rs = new PagerResult<Model>();
            using (var reader = await QueryMultipleAsync(countSql + pagerSql, parameterObject))
            {
                rs.TotalCount = reader.ReadFirst<int>();
                rs.Results = reader.Read<Model>();
            }
            return rs;
        }

        public async Task<Model> FirstOrDefaultAsync(string whereCmd, Model data)
        {
            var getAllCmd = CurrentSqlBuilder.GetSelectByFilterSql(whereCmd, 1);
            return await QueryFirstOrDefaultAsync<Model>(getAllCmd, data);
        }

        public async Task<Model> UpdateByGroupAndReturnAsync(string group, Model data)
        {
            var updateSql = CurrentSqlBuilder.GetUpdateSql(group);
            return await QuerySingleAsync<Model>(updateSql, data);
        }

        public async Task UpdateByGroupAsync(string group, Model data)
        {
            var updateSql = CurrentSqlBuilder.GetUpdateSql(group);
            await ExecuteAsync<Model>(updateSql, data);
        }

        public async Task InsertAsync(string group, Model data)
        {
            var insertSql = CurrentSqlBuilder.GetInsertSql(group);
            await ExecuteGetAffectAsync<Model>(insertSql, data);
        }

        public async Task<Model> InsertAndReturnAsync(string group, Model data)
        {
            var insertSql = CurrentSqlBuilder.GetInsertAndReturnSql(group );
            return await InsertAndReturnAsync<Model>(insertSql, data);
        }

        public async Task<dynamic> GetSingleOrDefaultByKeyAsync(Model data, string group)
        {
            var query = CurrentSqlBuilder.GetSelectByKeySql(group);
            return await QuerySingleOrDefaultDynamicAsync(query, data);
        }

        public async Task<dynamic> FirstOrDefaultAsync(string whereCmd, Model data, string group)
        {
            var query = CurrentSqlBuilder.GetSelectTopRecordsSql(1, group);
            return await QueryFirstOrDefaultDynamicAsync(query, data);
        }

        public async Task<dynamic> GetByKeyAsync(Model data, string group)
        {
            var query = CurrentSqlBuilder.GetSelectByKeySql(group);
            return await QuerySingleDynamicAsync(query, data);
        }

        public async Task<IEnumerable<dynamic>> GetAllAsync(string group)
        {
            var query = CurrentSqlBuilder.GetSelectAllSql(group);
            return await QueryDynamicAsync(query,null);
        }

        public async Task<IEnumerable<dynamic>> SearchAsync(string whereCmd, object parameterObject, string group)
        {
            var getAllCmd = CurrentSqlBuilder.GetSelectByFilterSql(whereCmd,group);
            return await QueryDynamicAsync(getAllCmd, parameterObject);
        }

        public async Task<IEnumerable<Model>> QueryTopAsync(Expression<Func<Model, bool>> exp, int number)
        {
            var sql = CurrentSqlBuilder.BuildQueryTop(exp, number);
            return await QueryAsync(sql);
        }
        public async Task<IEnumerable<Model>> QueryAsync(Expression<Func<Model, bool>> expression)
        {
            var sql = CurrentSqlBuilder.BuildQuery(expression);
            return await QueryAsync(sql);
        }

        public async Task<Model> FirstOrDefaultAsync(Expression<Func<Model, bool>> exp)
        {
            var sql = CurrentSqlBuilder.BuildQueryTop(exp, 1);
            return await FirstOrDefaultAsync(sql);
        }

        public async Task<PagerResult<Model>> PagerSearchAsync(FilterOptions<Model> opts)
        {
            var cmd =CurrentSqlBuilder.BuildQuery( opts);
            PagerResult<Model> rs = new PagerResult<Model>();
            using (var reader = await QueryMultipleAsync(cmd.Key, cmd.Value))
            {
                rs.TotalCount = reader.ReadFirst<int>();
                rs.Results = reader.Read<Model>();
            }
            return rs;
        }
    }

}
