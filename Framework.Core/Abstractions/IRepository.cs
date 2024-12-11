using System.Data.Common;
using System.Linq.Expressions;

namespace Framework.Core.Abstractions
{
    public interface IRepository<Model>
    {
        Task InsertAsync(Model data);

        Task InsertAsync(string group,Model data);

        Task InsertAsync(List<Model> models);
        Task<Model> InsertAndReturnAsync(Model data);

        Task<Model> InsertAndReturnAsync(string group, Model data);

        Task<Model> UpdateAndReturnAsync(Model data);

        Task UpdateAsync(Model data);

        Task<int> UpdateAsync(List<Model> models);

        Task DeleteAsync(Model data);

        Task DeleteAsync(List<Model> models);

        Task<Model> GetSingleOrDefaultByKeyAsync(Model data);

        Task<Model> FirstOrDefaultAsync(string whereCmd,Model data);
        Task DeleteAsync(Expression<Func<Model, bool>> exp);
        Task<Model> FirstOrDefaultAsync(Expression<Func<Model, bool>> expression);
        Task<IEnumerable<Model>> QueryTopAsync(Expression<Func<Model, bool>> exp, int number);

        Task<IEnumerable<Model>> QueryAsync(Expression<Func<Model, bool>> expression);
        Task<PagerResult<Model>> PagerSearchAsync(FilterOptions<Model> opts);

        Task<PagerResult<Model>> SearchAndCountByFilterOpts(FilterOptions opts, object parameterObject);

        Task<Model> GetByKeyAsync(Model data);

        Task<IEnumerable<Model>> GetAllAsync();

        Task<int> GetCountAsync(string whereCmd, object parameterObject);

        Task<IEnumerable<Model>> GetTopRecordsAsync(int number);

        Task<IEnumerable<Model>> SearchAsync(string whereCmd,object parameterObject);

        Task<IEnumerable<Model>> SearchAsync(string whereCmd, object parameterObject, int recordNumber);

        Task<IEnumerable<Model>> SearchAsync(string whereCmd, object parameterObject, int recordNumber, string orderBy);

        Task<IEnumerable<Model>> SearchByFilterOpts(FilterOptions opts, object parameterObject);

        Task<Model> UpdateByGroupAndReturnAsync(string group, Model data);

        Task UpdateByGroupAsync(string group, Model data);

        #region "return another model"
        Task<dynamic> GetSingleOrDefaultByKeyAsync(Model data,string group);

        Task<dynamic> FirstOrDefaultAsync(string whereCmd, Model data, string group);

        Task<dynamic> GetByKeyAsync(Model data, string group);

        Task<IEnumerable<dynamic>> GetAllAsync(string group);

        Task<IEnumerable<dynamic>> SearchAsync(string whereCmd, object parameterObject,string group);
        #endregion
    }
}
