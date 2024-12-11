using System;

namespace Framework.WebCore.Exceptions
{
    public class DataScopeNotPermittedException: Exception
    {
        public DataScopeNotPermittedException()
        {
        }

        public DataScopeNotPermittedException(string errorMessage) : base(errorMessage)
        {
        }
    }
}
