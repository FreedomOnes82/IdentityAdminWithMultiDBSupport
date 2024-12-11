using System;

namespace Framework.WebCore.Exceptions
{
    public class BusinessAcceptedException : Exception
    {
        public BusinessAcceptedException()
        {
        }

        public BusinessAcceptedException(string errorMessage) : base(errorMessage)
        {
        }
    }
}
