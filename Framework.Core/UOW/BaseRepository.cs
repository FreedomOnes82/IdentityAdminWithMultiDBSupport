using Dapper;
using Framework.Core.Abstractions;
using System.Data;
using static Framework.Core.ORM.SqlBuilderFactory;

namespace Framework.Core.UOW
{
    public abstract class BaseRepository<Model>
    {
        //protected IDbConnectionAccessor _dataAccessor;
        public BaseRepository(IDbConnection dbConn)
        {
            _dbConnection = dbConn;
        }

        public BaseRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        protected IUnitOfWork UnitOfWork
        {
            set;
            get;
        }

        private IDbConnection _dbConnection = null;
        protected IDbConnection DbConnection
        {
            get
            {
                if (UnitOfWork != null)
                {
                    return UnitOfWork.Connection;
                }
               
                return _dbConnection;
            }
        }

        protected virtual IDbTransaction DbTransaction
        {
            get
            {
                return UnitOfWork == null ? null : UnitOfWork.Transaction;
            }
        }

        protected async Task<int> ExecuteGetAffectAsync<Model>(string cmd, object data)
        {
            return await DbConnection.ExecuteAsync(cmd, data, DbTransaction);
        }

        protected async Task ExecuteAsync<Model>(string cmd, Model data)
        {
            await DbConnection.ExecuteAsync(cmd, data, DbTransaction);
        }

      
        protected async Task<Model> InsertAndReturnAsync<Model>(string insertAndReturnCmd, Model data)
        {
            return await DbConnection.QuerySingleAsync<Model>(insertAndReturnCmd, data, DbTransaction);
        }

        protected async Task<Model> UpdateAndReturnAsync<Model>(string updateCmd, Model data)
        {
            return await DbConnection.QuerySingleAsync<Model>(updateCmd, data, DbTransaction);
        }

        protected async Task<Model> QuerySingleAsync<Model>(string getByKeyCmd, Model data)
        {
            return await DbConnection.QuerySingleAsync<Model>(getByKeyCmd, data, DbTransaction);
        }

        protected async Task<R> QuerySingleOrDefaultAsync<Model, R>(string cmd, Model data)
        {
            return await DbConnection.QuerySingleOrDefaultAsync<R>(cmd, data, DbTransaction);
        }

        protected async Task<Model> QuerySingleOrDefaultAsync<Model>(string getByKeyCmd, Model data)
        {
            return await DbConnection.QuerySingleOrDefaultAsync<Model>(getByKeyCmd, data,DbTransaction);
        }

        protected async Task<Model> QueryFirstOrDefaultAsync<Model>(string query, Model data)
        {
            return await DbConnection.QueryFirstOrDefaultAsync<Model>(query, data, DbTransaction);
        }

        protected async Task<IEnumerable<Model>> QueryAsync<Model>(string query, Model model)
        {
            return await DbConnection.QueryAsync<Model>(query, model, DbTransaction);
        }

        protected async Task<IEnumerable<Model>> QueryAsync<Model>(string query, object model)
        {
            return await DbConnection.QueryAsync<Model>(query, model, DbTransaction);
        }

        protected async Task<SqlMapper.GridReader> QueryMultipleAsync(string query, object parameterObject)
        {
            return await DbConnection.QueryMultipleAsync(query, parameterObject, DbTransaction);
        }
        #region "Dynamic "

        protected async Task<dynamic> QuerySingleDynamicAsync<Model>(string getByKeyCmd, Model data)
        {
            return await DbConnection.QuerySingleAsync(getByKeyCmd, data, DbTransaction);
        }

        protected async Task<dynamic> QuerySingleOrDefaultDynamicAsync<Model>(string cmd, Model data)
        {
            return await DbConnection.QuerySingleOrDefaultAsync(cmd, data, DbTransaction);
        }

        protected async Task<dynamic> QueryFirstOrDefaultDynamicAsync<Model>(string query, Model data)
        {
            return await DbConnection.QueryFirstOrDefaultAsync(query, data, DbTransaction);
        }

        protected async Task<IEnumerable<dynamic>> QueryDynamicAsync<Model>(string query, Model model)
        {
            return await DbConnection.QueryAsync(query, model, DbTransaction);
        }

        protected async Task<IEnumerable<dynamic>> QueryDynamicAsync(string query, object model)
        {
            return await DbConnection.QueryAsync(query, model, DbTransaction);
        }
        #endregion

        #region "Linq Support"

        protected async Task<Model> QuerySingleAsync(KeyValuePair<string, DynamicParameters> cmd)
        {
            return await DbConnection.QuerySingleAsync<Model>(cmd.Key, cmd.Value,DbTransaction);
        }
        protected async Task<IEnumerable<Model>> QueryAsync(KeyValuePair<string, DynamicParameters> cmd)
        {
            return await DbConnection.QueryAsync<Model>(cmd.Key, cmd.Value, DbTransaction);
        }

        protected async Task<Model> FirstOrDefaultAsync(KeyValuePair<string, DynamicParameters> cmd)
        {
            return await DbConnection.QueryFirstOrDefaultAsync<Model>(cmd.Key, cmd.Value, DbTransaction);
        }

        private DBTypes _dbType = DBTypes.None;
        private DBTypes CurrentDbType
        {
            get
            {
                if (_dbType == DBTypes.None)
                {
                    var connectionType = DbConnection.GetType().FullName;
                        if (connectionType.Contains("Sqlite"))
                        {
                        _dbType = DBTypes.SQLITE;
                        }
                        else
                        {
                        if (connectionType.Contains("MySql", StringComparison.InvariantCultureIgnoreCase))
                            _dbType = DBTypes.MYSQL;
                        else
                            _dbType = DBTypes.SQLSERVER;
                        }
                }
                return _dbType;
            }
        }

      

        #endregion
    }
}
