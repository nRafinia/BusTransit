using Common.QMessageModels.SendMessages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using F4ST.Queue.Transmitters;

namespace Common.Transmitters
{
    public class QLogger : Transmitter, IQLogger
    {
        public async Task Trace(string logFileName, string message, string trackingCode, Dictionary<string, object> data = null,
            [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            await WriteLog(logFileName, LogType.Trace, message, trackingCode, data, sourceFilePath, methodName, lineNumber);
        }

        public async Task Debug(string logFileName, string message, string trackingCode, Dictionary<string, object> data = null,
            [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            await WriteLog(logFileName, LogType.Debug, message, trackingCode, data, sourceFilePath, methodName, lineNumber);
        }

        public async Task Info(string logFileName, string message, string trackingCode, Dictionary<string, object> data = null,
            [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            await WriteLog(logFileName, LogType.Info, message, trackingCode, data, sourceFilePath, methodName, lineNumber);
        }

        public async Task Error(string logFileName, string message, string trackingCode, Dictionary<string, object> data = null,
            [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            await WriteLog(logFileName, LogType.Error, message, trackingCode, data, sourceFilePath, methodName, lineNumber);
        }

        public async Task Error(string logFileName, Exception exception, string trackingCode, Dictionary<string, object> data = null,
            [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (data == null)
                data = new Dictionary<string, object>();

            var lst = data.ToList();
            lst.Insert(0, new KeyValuePair<string, object>("Exception", exception));

            data = lst.ToDictionary(d => d.Key, d => d.Value);

            await WriteLog(logFileName, LogType.Error, exception.Message, trackingCode, data, sourceFilePath, methodName, lineNumber);
        }

        private async Task WriteLog(string logFileName, LogType logType, string message, string trackingCode,
            Dictionary<string, object> data, string sourceFilePath, string methodName, int lineNumber)
        {
            /*await Send("Logger_Queue", new QLogMessage()
            {
                LogType = logType,
                LogTime = DateTime.Now,
                LogFileName = logFileName,
                TrackingCode = trackingCode,
                ProcessId = Process.GetCurrentProcess().Id,
                SourceFile = sourceFilePath,
                MethodName = methodName,
                LineNumber = lineNumber,
                Message = message,
                Data = data
            });*/
        }
    }
}