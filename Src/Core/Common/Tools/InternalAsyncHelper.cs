using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Common.Tools
{
    internal static class InternalAsyncHelper
    {
        public static async Task AwaitTaskWithPostActionAndFinally(Task actualReturnValue, Func<Task> postAction,
            Action<Exception> finalAction)
        {
            Exception exception = null;

            try
            {
                await actualReturnValue;
                if (postAction != null)
                    await postAction();
            }
            catch (Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                finalAction(exception);
            }
        }

        public static async Task<T> AwaitTaskWithPostActionAndFinallyAndGetResult<T>(Task<T> actualReturnValue,
            Func<Task> postAction, Action<Exception> finalAction)
        {
            Exception exception = null;

            try
            {
                var result = await actualReturnValue;
                if (postAction != null)
                    await postAction();
                return result;
            }
            catch (Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                finalAction(exception);
            }
        }

        public static object CallAwaitTaskWithPostActionAndFinallyAndGetResult(Type taskReturnType,
            object actualReturnValue, Func<Task> action, Action<Exception> finalAction)
        {
            return typeof(InternalAsyncHelper)
                .GetMethod("AwaitTaskWithPostActionAndFinallyAndGetResult", BindingFlags.Public | BindingFlags.Static)
                ?.MakeGenericMethod(taskReturnType)
                .Invoke(null, new object[] {actualReturnValue, action, finalAction});
        }
    }
}