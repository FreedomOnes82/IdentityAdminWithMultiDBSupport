using Framework.Core.Abstractions;
using Framework.Core.ORM;
using static Framework.Core.ORM.SqlBuilderFactory;
using System.Data;

namespace Framework.Core.UOW
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Model"></typeparam>
    public class PartitionedRepository<Model> : BaseRepository<Model>, IPartitionRepository<Model> where Model : PartitionModelBase
    {
        protected IDbConnection _connection;
        DBTypes _dbType = DBTypes.None;
        ISqlBuilderPartition _sqlBuilder = null;

        public PartitionedRepository(IDbConnection db, IDataTablePrefixProvider dataTablePrefixProvider,
            IUpdateCommandGroupProvider updateCmdGroupProvider) : base(db)
        {
            InitSqlBuilder(DbConnection, dataTablePrefixProvider, updateCmdGroupProvider);
        }

        public PartitionedRepository(IUnitOfWork uow, IDataTablePrefixProvider dataTablePrefixProvider,
            IUpdateCommandGroupProvider updateCmdGroupProvider) : base(uow)
        {
            InitSqlBuilder(DbConnection, dataTablePrefixProvider, updateCmdGroupProvider);
        }

        private void InitSqlBuilder(IDbConnection dbConn, IDataTablePrefixProvider dataTablePrefixProvider, 
            IUpdateCommandGroupProvider updateCmdGroupProvider)
        {
            var connectionType = DbConnection.GetType().FullName;
            if (_sqlBuilder == null)
                if (connectionType.Contains("Sqlite"))
                {
                    _sqlBuilder = SqlBuilderFactory.CreatePartitionInstance<Model>(DBTypes.SQLITE,
                        dataTablePrefixProvider, updateCmdGroupProvider);
                }
                else
                {
                    if (connectionType.Contains("MySql", StringComparison.InvariantCultureIgnoreCase))
                        _sqlBuilder = SqlBuilderFactory.CreatePartitionInstance<Model>(DBTypes.MYSQL, 
                            dataTablePrefixProvider, updateCmdGroupProvider);
                    else
                        _sqlBuilder = SqlBuilderFactory.CreatePartitionInstance<Model>(DBTypes.SQLSERVER,
                            dataTablePrefixProvider, updateCmdGroupProvider);
                }
        }

        private ISqlBuilderPartition CurrentSqlBuilder
        {
            get
            {
                return _sqlBuilder;
            }
        }

        public async Task InsertAsync(Model data)
        {
            var insertSql = CurrentSqlBuilder.GetInsertSql<Model>(data);
            await ExecuteGetAffectAsync<Model>(insertSql, data);
        }


        public async Task<Model> InsertAndReturnAsync(Model data)
        {
            var insertSql = CurrentSqlBuilder.GetInsertAndReturnSql<Model>(data);
            return await QuerySingleOrDefaultAsync(insertSql, data);
        }

        public async Task<Model> UpdateAndReturnAsync(Model data)
        {
            var updateAndReturnSql = CurrentSqlBuilder.GetUpdateAndReturnSql<Model>(data);
            return await QuerySingleOrDefaultAsync(updateAndReturnSql, data);
        }

        public async Task ExecuteCmdAsyn(string updateCmd, Model data)
        {
            await ExecuteAsync(updateCmd, data);
        }

        public async Task UpdateAsync(Model data)
        {
            var updateSql = CurrentSqlBuilder.GetUpdateSql<Model>(data);
            await ExecuteCmdAsyn(updateSql, data);
        }

        public async Task UpdateAsync(string group, Model data)
        {
            var updateSql = CurrentSqlBuilder.GetUpdateSql<Model>(group,data);
            await ExecuteCmdAsyn(updateSql, data);
        }

        public async Task<Model> UpdateAndReturnAsync(string group,Model data)
        {
            var updateAndReturnSql = CurrentSqlBuilder.GetUpdateAndReturnSql<Model>(group,data);
            return await QuerySingleOrDefaultAsync(updateAndReturnSql, data);
        }

        public async Task DeleteAsync(Model data)
        {
            var updateSql = CurrentSqlBuilder.GetDeleteByKey<Model>(data);
            await ExecuteCmdAsyn(updateSql, data);
        }

        private async Task<Model> GetSingleOrDefaultByCmdAsync(string getByKeyCmd, Model data)
        {
            return await QuerySingleOrDefaultAsync<Model>(getByKeyCmd, data);
        }
        public async Task<Model> GetSingleOrDefaultByKeyAsync(Model data)
        {
            var getByKeyCmd = CurrentSqlBuilder.GetSelectByKeySql<Model>(data);
            return await GetSingleOrDefaultByCmdAsync(getByKeyCmd, data);
        }
        public async Task<Model> GetByKeyAsync(Model data)
        {
            var getByKeyCmd = CurrentSqlBuilder.GetSelectByKeySql<Model>(data);
            return await QuerySingleAsync(getByKeyCmd, data);
        }

        public async Task InsertAsync(string group, Model data)
        {
            var insertSql = CurrentSqlBuilder.GetInsertSql<Model>(group,data);
            await ExecuteGetAffectAsync<Model>(insertSql, data);
        }

        public async Task<Model> InsertAndReturnAsync(string group, Model data)
        {
            var insertSql = CurrentSqlBuilder.GetInsertAndReturnSql<Model>(group,data);
            return await InsertAndReturnAsync<Model>(insertSql, data);
        }

        public async Task<dynamic> GetSingleOrDefaultDynamicByKeyAsync(Model data, string group)
        {
            var getByKeyCmd = CurrentSqlBuilder.GetSelectByKeySql<Model>(data, group);
            //return await GetSingleOrDefaultByCmdAsync(getByKeyCmd, data);
            return await QuerySingleOrDefaultDynamicAsync(getByKeyCmd, data);
        }

        public async Task<dynamic> GetDynamicByKeyAsync(Model data, string group)
        {
            var getByKeyCmd = CurrentSqlBuilder.GetSelectByKeySql<Model>(data, group);
            //return await GetSingleOrDefaultByCmdAsync(getByKeyCmd, data);
            return await QuerySingleDynamicAsync(getByKeyCmd, data);
        }
    }

}
