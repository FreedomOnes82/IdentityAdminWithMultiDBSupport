namespace Framework.Core.Abstractions
{
    public interface IUnitOfWorkFactory
    {
        public IUnitOfWork CreateUnitOfWork();
    }
}
