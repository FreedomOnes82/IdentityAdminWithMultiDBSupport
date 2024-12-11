using System;

namespace Framework.WebCore.Exceptions
{
    public class RoleNotPermittedException : Exception
    {
        public RoleNotPermittedException()
        {
        }

        public RoleNotPermittedException(string errorMessage) : base(errorMessage)
        {
        }
    }
}
