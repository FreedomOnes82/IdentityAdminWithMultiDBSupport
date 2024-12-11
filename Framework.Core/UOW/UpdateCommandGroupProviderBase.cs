using Framework.Core.Abstractions;

namespace Framework.Core.UOW
{
    public class UpdateCommandGroupProviderBase : IUpdateCommandGroupProvider
    {
        public List<string> GetUpdateFields(string tableName, string groupName)
        {
            return new List<string>();
        }
    }
}
//IUpdateCommandGroupProvider