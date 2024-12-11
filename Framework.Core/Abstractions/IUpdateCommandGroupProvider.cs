namespace Framework.Core.Abstractions
{
    [Obsolete]
    public interface IUpdateCommandGroupProvider
    {
        List<string> GetUpdateFields(string tableName, string groupName);
    }
}
