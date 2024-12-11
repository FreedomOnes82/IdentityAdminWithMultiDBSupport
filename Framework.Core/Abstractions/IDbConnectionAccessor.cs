using System.Data.Common;

namespace Framework.Core.Abstractions
{
    public interface IDbConnectionAccessor
    {
        DbConnection CreateConnection();
    }
}
