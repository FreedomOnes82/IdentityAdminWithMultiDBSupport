using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Framework.Core.ORM
{
    public  class PreCompileGetterHelper<T>
    {
        private static ConcurrentDictionary<string, Func<T, object>> _getters = null;
        public static ConcurrentDictionary<string, Func<T, object>> Getters 
        { 
            get 
            {
                if (_getters == null)
                {
                    _getters = new ConcurrentDictionary<string, Func<T, object>>();
                    var properties = typeof(T).GetProperties();
                    foreach (var property in properties)
                    {
                        _getters.TryAdd(property.Name, CompileGetter(property.Name));
                    }
                }
                return _getters;
            } 
        } 

        private static Func<T, object> CompileGetter(string propName)
        {
            // create Expression parameter
            var param = Expression.Parameter(typeof(T), "obj");

            // create Expression tree
            var propertyExpr = Expression.Property(param, propName);

            // creat Lambda expression
            var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(propertyExpr, typeof(object)), param);

            // compile delegate
            var getter = lambda.Compile();
            return getter;
        }
    }
}
