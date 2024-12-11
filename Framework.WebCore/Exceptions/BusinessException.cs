using System;

namespace Framework.WebCore.Exceptions
{
    /// <summary>
    ///
    /// </summary>
    public class BusinessException:Exception
    {
        public BusinessException()
        { 
        }

        public BusinessException(string errorMessage) : base(errorMessage)
        { 
        }
    }
}
