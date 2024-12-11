using Framework.Core.Abstractions;
using Framework.Core.UOW;

namespace Framework.Core.Services
{
    public class ServiceBase : IServiceBase
    {
        private IDbConnectionAccessor _dataAccesser;
        private readonly IDataTablePrefixProvider _dataTablePrefixProvider;
        private readonly IUpdateCommandGroupProvider _updateCmdGroupProvider;
        private IUnitOfWork _unitOfWork;
        protected IUnitOfWorkFactory _unitOfWorkFactory;

        public ServiceBase(IDbConnectionAccessor dbConnAccessor,IDataTablePrefixProvider dataTablePrefixProvider, 
            IUpdateCommandGroupProvider updateCmdGroupProvider)
        {
            _dataAccesser = dbConnAccessor;
            _dataTablePrefixProvider = dataTablePrefixProvider;
            _updateCmdGroupProvider = updateCmdGroupProvider;
        }

        public ServiceBase(IDbConnectionAccessor dbConnAccessor)
        {
            _dataAccesser = dbConnAccessor;
        }


        protected IUnitOfWork CreateUnitOfWork()
        {
            var uow = new UnitOfWork(_dataAccesser.CreateConnection(), _dataTablePrefixProvider,_updateCmdGroupProvider);
            CurrentUnitOfWork = uow;
            return uow;
        }

        public IUnitOfWork CurrentUnitOfWork 
        {
            get
            {
                return _currentUnitOfWork;
            }
            set 
            {
                _currentUnitOfWork = value;
                _currentUnitOfWork.AddReferredService(this);
            }
        }
        protected IUnitOfWork _currentUnitOfWork = null;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <returns></returns>
        public IRepository<Model> GetRepository<Model>() where Model : class
        {
            if (_currentUnitOfWork != null)
                return _currentUnitOfWork.GetRepository<Model>();
            return new Repository<Model>(_dataAccesser.CreateConnection(),
                _dataTablePrefixProvider, _updateCmdGroupProvider);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <returns></returns>
        public IPartitionRepository<Model> GetPartitionRepository<Model>() where Model : PartitionModelBase
        {
            if (_currentUnitOfWork != null)
                return _currentUnitOfWork.GetPartitionRepository<Model>();
            return new PartitionedRepository<Model>(_dataAccesser.CreateConnection(), 
                _dataTablePrefixProvider, _updateCmdGroupProvider);
        }

        public async Task ExecuteInDbSession(Func<IUnitOfWork, Task> func)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                await func.Invoke(unitOfWork);
            }
        }

        public async Task<Model> ExecuteInDbSession<Model>(Func<IUnitOfWork, Task<Model>> func)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await func.Invoke(unitOfWork);
            }
        }

        public async Task<Model> ExecuteInTransaction<Model>(Func<IUnitOfWork, Task<Model>> func)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var rs = await func.Invoke(unitOfWork);
                unitOfWork.SaveChanges();
                return rs;
            }
        }

        public void ClearUnitOfWork()
        {
            _currentUnitOfWork = null;
        }

        //public string CurrentUserId { get; set; }
    }
}
