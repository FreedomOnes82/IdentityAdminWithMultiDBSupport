namespace Framework.Core.Abstractions
{
    public interface IPartitionRepository<Model> where Model:PartitionModelBase
    {
        Task InsertAsync(Model data);
        Task<Model> InsertAndReturnAsync(Model data);
        Task<Model> UpdateAndReturnAsync(Model data);
        Task ExecuteCmdAsyn(string updateCmd, Model data);
        Task UpdateAsync(Model data);
        Task DeleteAsync(Model data);
        Task<Model> GetSingleOrDefaultByKeyAsync(Model data);
        Task<Model> GetByKeyAsync(Model data);

        Task<Model> UpdateAndReturnAsync(string group,Model data);
        Task UpdateAsync(string group, Model data);

        Task InsertAsync(string group, Model data);
        Task<Model> InsertAndReturnAsync(string group,Model data);

        #region "return another model"
        Task<dynamic> GetSingleOrDefaultDynamicByKeyAsync(Model data, string group);


        Task<dynamic> GetDynamicByKeyAsync(Model data, string group);
        #endregion
    }
}
