using System.Collections.Generic;

namespace Common.QMessageModels.RequestMessages
{
    public class QWebResponse : QBaseResponse
    {
        /// <summary>
        /// کد منحصر بفرد برای پیگیری
        /// </summary>
        public string TraceId { get; set; }

        /// <summary>
        /// هدرهای پاسخ
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// کد بازگشتی
        /// </summary>
        public int Status { get; set; } = 200;

        /// <summary>
        /// پاسخ ارسالی
        /// </summary>
        public string Response { get; set; }
    }
}