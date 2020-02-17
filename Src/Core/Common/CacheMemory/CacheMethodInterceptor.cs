using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Common.Attributes;
using Common.Tools;
using Castle.DynamicProxy;
using F4ST.Common.Containers;
using F4ST.Common.Tools;

namespace Common.CacheMemory
{
    public class CacheMethodInterceptor : BaseInterceptorASync
    {
        private readonly ICachingKeyGenerator _keyGenerator;
        private readonly ICacheMemory _cacheMemory;

        /*private MethodInfo _serviceMethod;
        private string _cacheKey;
        private CacheOutputAttribute _attribute;*/

        public CacheMethodInterceptor(ICachingKeyGenerator keyGenerator, ICacheMemory cacheMemory)
        {
            _keyGenerator = keyGenerator;
            _cacheMemory = cacheMemory;
        }

        protected override bool BeforeRunMethod(IInvocation invocation)
        {
            var serviceMethod = invocation.MethodInvocationTarget ?? invocation.Method;

            if (!(serviceMethod.GetCustomAttributes(true)
                    .FirstOrDefault(x => x.GetType() == typeof(CacheOutputAttribute)) is CacheOutputAttribute
                attribute))
                return true;

            var returnType = serviceMethod.ReturnType;
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                returnType = returnType.GenericTypeArguments[0];

                var cacheKey = _keyGenerator.GetCacheKey(serviceMethod, invocation.Arguments, attribute.CacheKeyPrefix, returnType);

            var cacheValue = AsyncHelpers.RunSync(() => _cacheMemory.Get(returnType, cacheKey));

            if (cacheValue == null)
                return true;

            invocation.ReturnValue = cacheValue;

            if (!IsAsyncMethod(serviceMethod))
            {
                return false;
            }

            if (invocation.Method.ReturnType == typeof(Task))
            {
                invocation.ReturnValue = GetTaskResult();
                return false;
            }
            var m = GetType().GetMethod("GetGenericResult");
            var g = m?.MakeGenericMethod(invocation.Method.ReturnType.GenericTypeArguments[0]);
            invocation.ReturnValue = g?.Invoke(this, new[] { invocation.ReturnValue });

            //invocation.ReturnValue = cacheValue;
            return false;
        }

        protected override void AfterRunMethod(IInvocation invocation)
        {
            var serviceMethod = invocation.MethodInvocationTarget ?? invocation.Method;
            if (!(serviceMethod.GetCustomAttributes(true)
                    .FirstOrDefault(x => x.GetType() == typeof(CacheOutputAttribute)) is CacheOutputAttribute
                attribute))
            {
                return;
            }

            var cacheKey = _keyGenerator.GetCacheKey(serviceMethod, invocation.Arguments, attribute.CacheKeyPrefix);

            if (string.IsNullOrWhiteSpace(cacheKey) || invocation.ReturnValue == null)
                return;

            var value = invocation.ReturnValue;

            if (value.GetType().IsGenericType && value.GetType().GetGenericTypeDefinition() == typeof(Task<>))
            {
                var result = value.GetType().GetProperty("Result")?.GetValue(value, null);
                cacheKey = _keyGenerator.GetCacheKey(serviceMethod, invocation.Arguments, attribute.CacheKeyPrefix,result?.GetType());
                value = result;
            }

            _cacheMemory.Set(cacheKey, value, attribute.Expiration);
        }

        private static bool IsAsyncMethod(MethodInfo method)
        {
            return (
                method.ReturnType == typeof(Task) ||
                (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            );
        }


        protected override void OnException(IInvocation invocation, Exception ex)
        {
        }
    }
}