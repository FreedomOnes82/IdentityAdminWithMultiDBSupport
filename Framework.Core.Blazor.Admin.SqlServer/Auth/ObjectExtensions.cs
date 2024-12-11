using System;

namespace Framework.Core.Blazor.Admin.SqlServer.Auth
{
    internal static class ObjectExtensions
    {
        internal static void ThrowIfNull<T>(this T @object, string paramName) {
            if (@object == null) {
                throw new ArgumentNullException(paramName, $"Parameter {paramName} cannot be null.");
            }
        }
    }
}
