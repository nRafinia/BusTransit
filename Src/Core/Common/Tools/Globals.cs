using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using F4ST.Common.Tools;

namespace Common.Tools
{
    public static class Globals
    {
        public static IEnumerable<T> GetImplementedInterfaceOf<T>()
        {
            var res = new List<T>();
            var ass = System.Reflection.Assembly.GetEntryAssembly();

            if (ass == null)
                return res;

            foreach (var ti in ass.DefinedTypes)
            {
                if (ti.ImplementedInterfaces.Contains(typeof(T)))
                {
                    res.Add((T)ass.CreateInstance(ti.FullName));
                }
            }

            return res;
        }

        /// <summary>
        /// اجرای متود و منتظر بودم برای پاسخ
        /// </summary>
        /// <param name="sender">کلاس اجرا کننده</param>
        /// <param name="targetMethod">متود مربوطه</param>
        /// <param name="args">پارامترها</param>
        /// <param name="haveResult">آیا مقدار بازگشتی دارد یا خیر</param>
        /// <returns>مقدار بازگشتی متود</returns>
        public static object RunMethod(object sender, MethodInfo targetMethod, object[] args, bool haveResult)
        {
            var res = targetMethod.Invoke(sender, parameters: args);
            if (!haveResult)
                return null;

            var returnType = targetMethod.ReturnType;
            if (returnType != typeof(Task) &&
                !(returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>)))
            {
                return res;
            }

            var task = (Task)res;
            AsyncHelpers.RunSync(() => task);

            if (returnType == typeof(Task))
            {
                return null;
            }

            var result = task.GetType().GetProperty("Result")?.GetValue(task, null);
            return result;
        }
    }
}