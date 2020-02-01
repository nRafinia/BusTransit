using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common.CacheMemory
{
    public class CachingKeyGenerator : ICachingKeyGenerator
    {
        /// <summary>
        /// The link char of cache key.
        /// </summary>
        private const char LinkChar = ':';

        /// <inheritdoc />
        public string GetCacheKey(MethodInfo methodInfo, object[] args, string prefix, Type returnType = null)
        {
            IList<string> methodArguments;
            if (string.IsNullOrWhiteSpace(prefix))
            {
                if (methodInfo.DeclaringType != null)
                {
                    var typeName = (returnType ?? methodInfo.DeclaringType).Name;
                    var methodName = methodInfo.Name;

                    methodArguments = this.FormatArgumentsToPartOfCacheKey(args);

                    return this.GenerateCacheKey(typeName, methodName, methodArguments);
                }
            }

            methodArguments = this.FormatArgumentsToPartOfCacheKey(args);
            return this.GenerateCacheKey(string.Empty, prefix, methodArguments);
        }

        /// <inheritdoc />
        public string GetCacheKeyPrefix(MethodInfo methodInfo, string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                if (methodInfo.DeclaringType != null)
                {
                    var typeName = methodInfo.DeclaringType.Name;
                    var methodName = methodInfo.Name;

                    return this.GenerateCacheKeyPrefix(typeName, methodName);
                }
            }

            return this.GenerateCacheKeyPrefix(string.Empty, prefix);
        }

        /// <summary>
        /// Generates the cache key prefix.
        /// </summary>
        /// <returns>The cache key prefix.</returns>
        /// <param name="first">First.</param>
        /// <param name="second">Second.</param>
        private string GenerateCacheKeyPrefix(string first, string second)
        {
            return string.Concat(first, LinkChar, second, LinkChar).TrimStart(LinkChar);
        }

        /// <summary>
        /// Formats the arguments to part of cache key.
        /// </summary>
        /// <returns>The arguments to part of cache key.</returns>
        /// <param name="methodArguments">Method arguments.</param>
        private IList<string> FormatArgumentsToPartOfCacheKey(object[] methodArguments)
        {
            if (methodArguments != null && methodArguments.Length > 0)
            {
                return methodArguments.Select(this.GetArgumentValue).ToList();
            }
            else
            {
                return new List<string> {"0"};
            }
        }

        /// <summary>
        /// Generates the cache key.
        /// </summary>
        /// <returns>The cache key.</returns>
        /// <param name="typeName">Type name.</param>
        /// <param name="methodName">Method name.</param>
        /// <param name="parameters">Parameters.</param>
        private string GenerateCacheKey(string typeName, string methodName, IList<string> parameters)
        {
            var builder = new StringBuilder();

            builder.Append(this.GenerateCacheKeyPrefix(typeName, methodName));

            foreach (var param in parameters)
            {
                builder.Append(param);
                builder.Append(LinkChar);
            }

            var str = builder.ToString().TrimEnd(LinkChar);

            return str;
            //using (SHA1 sha1 = SHA1.Create())
            //{
            //    byte[] data = sha1.ComputeHash(Encoding.UTF8.GetBytes(str));
            //    return Convert.ToBase64String(data, Base64FormattingOptions.None);
            //}
        }

        /// <summary>
        /// Gets the argument value.
        /// </summary>
        /// <returns>The argument value.</returns>
        /// <param name="arg">Argument.</param>
        private string GetArgumentValue(object arg)
        {
            if (arg is int || arg is long || arg is string)
                return arg.ToString();

            if (arg is DateTime time)
                return time.ToString("yyyyMMddHHmmss");

            return null;
        }
    }
}