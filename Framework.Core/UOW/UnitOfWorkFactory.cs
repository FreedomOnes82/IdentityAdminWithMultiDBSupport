using Framework.Core.Abstractions;

namespace Framework.Core.UOW
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private IDbConnectionAccessor _dbConnectionAccessor;
        private readonly IDataTablePrefixProvider _dataTablePrefixProvider;

        public UnitOfWorkFactory(IDbConnectionAccessor dbConnectionAccessor, IDataTablePrefixProvider dataTablePrefixProvider)
        {
            _dbConnectionAccessor = dbConnectionAccessor;
            _dataTablePrefixProvider = dataTablePrefixProvider;
        }

        private IUnitOfWork _unitOfWork;

        public IUnitOfWork CreateUnitOfWork()
        {
            //if (_unitOfWork == null || _unitOfWork.Connection == null)
            //{ 
            //    _unitOfWork =  new UnitOfWork(_dbConnectionAccessor, _dataTablePrefixProvider);
            //}
            return _unitOfWork;   
        }
    }
}
