namespace Framework.Core.Abstractions
{
    public interface IServiceBase
    {
        // string CurrentUserId { get; set; }
        void ClearUnitOfWork();
    }
}
