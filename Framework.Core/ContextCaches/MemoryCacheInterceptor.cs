using Castle.Core.Internal;
using Castle.DynamicProxy;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Framework.Core.ContextCaches
{
    public class MemoryCacheInterceptor : IInterceptor
    {
        private IContextCache _contextCache;

        public MemoryCacheInterceptor(IContextCache contextCache)
        {
            _contextCache = contextCache;
        }

        private bool CheckMethodReturnTypeIsTaskType(MethodInfo method)
        {
            var methodReturnType = method.ReturnType;
            if (methodReturnType.IsGenericType)
            {
                if (methodReturnType.GetGenericTypeDefinition() == typeof(Task<>) ||
                    methodReturnType.GetGenericTypeDefinition() == typeof(ValueTask<>))
                    return true;
            }
            else
            {
                if (methodReturnType == typeof(Task) ||
                    methodReturnType == typeof(ValueTask))
                    return true;
            }
            return false;
        }

        private bool IsAsyncMethod(MethodInfo method)
        {
            bool isDefAsync = Attribute.IsDefined(method, typeof(AsyncStateMachineAttribute), false);
            bool isTaskType = CheckMethodReturnTypeIsTaskType(method);
            bool isAsync = isDefAsync && isTaskType;

            return isAsync;
        }

        public void Intercept(IInvocation invocation)
        {
            
            MethodInfo method = invocation.Method;
            var attributes = method.GetAttributes<ContextCacheAttribute>();
            if (attributes != null && attributes.Count() > 0)
            {
                ContextCacheAttribute attr = attributes.First() as ContextCacheAttribute;

                if (attr != null)
                {
                    string cacheKey = attr.Key;
                    bool isRemoval = attr.IsRemoval;
                    if (isRemoval)
                    {
                        invocation.Proceed();
                        _contextCache.Keys.Remove(cacheKey);
                        _contextCache.Remove(cacheKey);
                        return;
                    }

                    if (_contextCache.Keys.Contains(cacheKey))
                    {
                        var cachedObject = _contextCache.Get(cacheKey);
                        if (cachedObject != null)
                        {
                            invocation.ReturnValue = cachedObject;
                            return;
                        }
                    }

                    invocation.Proceed();
                    if (IsAsyncMethod(invocation.MethodInvocationTarget))
                    {
                        invocation.ReturnValue = InterceptAsync((dynamic)invocation.ReturnValue, invocation);
                    }
                    else
                    {
                        AfterProceedSync(invocation);
                    }
                    _contextCache.Set(cacheKey, invocation.ReturnValue);
                }
            }
            else
            {
                invocation.Proceed();
            }
        }

        protected object ProceedAsyncResult { get; set; }


        private async Task InterceptAsync(Task task, IInvocation invocation)
        {
            await task.ConfigureAwait(false);
            await AfterProceedAsync(invocation, false);
        }

        private async Task<TResult> InterceptAsync<TResult>(Task<TResult> task, IInvocation invocation)
        {
            ProceedAsyncResult = await task.ConfigureAwait(false);
            await AfterProceedAsync(invocation, true);
            return (TResult)ProceedAsyncResult;
        }

        private async ValueTask InterceptAsync(ValueTask task, IInvocation invocation)
        {
            await task.ConfigureAwait(false);
            await AfterProceedAsync(invocation, false);
        }

        private async ValueTask<TResult> InterceptAsync<TResult>(ValueTask<TResult> task, IInvocation invocation)
        {
            ProceedAsyncResult = await task.ConfigureAwait(false);
            await AfterProceedAsync(invocation, true);
            return (TResult)ProceedAsyncResult;
        }

        protected virtual void BeforeProceed(IInvocation invocation) { }

        protected virtual void AfterProceedSync(IInvocation invocation) { }

        protected virtual Task AfterProceedAsync(IInvocation invocation, bool hasAsynResult)
        {
            return Task.CompletedTask;
        }
    }
}
