using Castle.Components.DictionaryAdapter;
using Framework.Core.Abstractions;
using Framework.Core.Abstractions.Dtos;

namespace Framework.Core.Services
{
    public abstract class CRUDServiceBase<CreationDto, ModifyDto, ReturnDto,DataModel> :ServiceBase//,ICRUDServiceBase<CreationDto, ModifyDto, ReturnDto>
        where DataModel : class
        where ReturnDto : class
        where ModifyDto : IModifyDto<DataModel>
        where CreationDto : ICreationDto<DataModel>
    {

        public CRUDServiceBase(IDbConnectionAccessor dbConnectionAccessor, IDataTablePrefixProvider dataTablePrefixProvider,
            IUpdateCommandGroupProvider updateCommandGroupProvider):
            base(dbConnectionAccessor,dataTablePrefixProvider, updateCommandGroupProvider)
        {
        }

        protected virtual ReturnDto ConvertDbModelToReturnDto(DataModel dbModel)
        { 
            throw new NotImplementedException();
        }

        protected virtual string GetCurrentUserId()
        { 
            throw new NotImplementedException();
        }

        public async virtual Task<ReturnDto?> Create(CreationDto dto)
        {
            var dbModel = await base.GetRepository<DataModel>()
                .InsertAndReturnAsync(dto.ConvertToDbModel(GetCurrentUserId()));
            return ConvertDbModelToReturnDto(dbModel);
        }

        protected async virtual Task Delete(DataModel model)
        {
            await base.GetRepository<DataModel>()
             .DeleteAsync(model);
        }

        public async virtual Task<IEnumerable<ReturnDto>> GetAll()
        {
            var dbModels = await base.GetRepository<DataModel>()
              .GetAllAsync();
            return dbModels.Select(x => ConvertDbModelToReturnDto(x));
        }

        protected async virtual Task<ReturnDto?> GetByKey(DataModel model)
        {
            var dbModel = await base.GetRepository<DataModel>()
               .GetByKeyAsync(model!);
            return ConvertDbModelToReturnDto(dbModel);
        }

        protected async Task<DataModel> GetDataModelByKey(DataModel model)
        {
            var dbModel = await base.GetRepository<DataModel>()
               .GetByKeyAsync(model!);
            return dbModel;
        }

        protected async virtual Task<ReturnDto> Update(DataModel existingRecord, ModifyDto dto)
        {
            var dbModel = await base.GetRepository<DataModel>()
                .UpdateAndReturnAsync(dto.ConvertToDbModel(existingRecord,GetCurrentUserId()));
            return ConvertDbModelToReturnDto(dbModel);
        }

        public async virtual Task<ReturnDto> Update(ModifyDto dto)
        { 
            var existingRecord = await GetDataModelByKey(dto.ConvertToDbModelWithKeyOnly());
            return await Update(existingRecord, dto);
        }

        public async virtual Task DeleteByKey(ModifyDto dto)
        {
             await Delete(dto.ConvertToDbModelWithKeyOnly());
        }
    }
}
