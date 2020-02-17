using System;
using System.Collections.Generic;
using F4ST.Queue.QMessageModels;

namespace Common.QMessageModels.SendMessages
{
    public class QLogMessage : QBaseMessage
    {
        /// <summary>
        /// نوع لاگ
        /// </summary>
        public LogType LogType { get; set; }

        /// <summary>
        /// زمان رخ دادن
        /// </summary>
        public DateTime LogTime { get; set; } = DateTime.Now;

        /// <summary>
        /// نام فایل لاگ
        /// </summary>
        public string LogFileName { get; set; }

        /// <summary>
        /// کد پیگیری لاگ
        /// </summary>
        public string TrackingCode { get; set; }

        /// <summary>
        /// Process Id
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// آدرس فایل لاگ کننده
        /// </summary>
        public string SourceFile { get; set; }

        /// <summary>
        /// نام متود لاگ کننده
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// شماره خط لاگ کننده
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// متن لاگ
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// سایر اطلاعات
        /// </summary>
        public Dictionary<string, object> Data { get; set; } = null;
    }
}