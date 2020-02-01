using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Common.Transmitters
{
    public interface IQLogger
    {
        Task Trace(string logFileName, string message, string trackingCode, Dictionary<string, object> data = null,
            [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int lineNumber = 0);

        Task Debug(string logFileName, string message, string trackingCode, Dictionary<string, object> data = null,
            [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int lineNumber = 0);

        Task Info(string logFileName, string message, string trackingCode, Dictionary<string, object> data = null,
            [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int lineNumber = 0);

        Task Error(string logFileName, string message, string trackingCode, Dictionary<string, object> data = null,
            [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int lineNumber = 0);

        Task Error(string logFileName, Exception exception, string trackingCode, Dictionary<string, object> data = null,
            [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int lineNumber = 0);
    }
}