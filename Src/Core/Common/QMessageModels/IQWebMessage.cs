using System.Collections.Generic;
using System.Linq;
using Common.Models;

namespace Common.QMessageModels
{
    public interface IQWebMessage
    {
        /// <summary>
        /// کد منحصر بفرد برای پیگیری
        /// </summary>
        string TraceId { get; set; }

        /// <summary>
        /// http / https
        /// </summary>
        string Scheme { get; set; }

        /// <summary>
        /// آدرس دامین
        /// </summary>
        string Domain { get; set; }

        /// <summary>
        /// آدرس اصلی سایت
        /// </summary>
        string BasePath { get; set; }

        /// <summary>
        /// آدرس کامل سایت
        /// </summary>
        string BaseUrl { get; }

        /// <summary>
        /// قسمت اول آدرس درخواستی که نام queue میباشد
        /// </summary>
        string Target { get; }

        /// <summary>
        /// آدرس درخواستی
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// IP کاربر
        /// </summary>
        string IP { get; set; }

        /// <summary>
        /// نوع درخواست Get, Post,...
        /// </summary>
        string HttpMethod { get; set; }

        /// <summary>
        /// هدرهای درخواست
        /// </summary>
        Dictionary<string, string[]> Headers { get; set; }

        /// <summary>
        /// مقادیر ارسالی
        /// </summary>
        IDictionary<string, object> Arguments { get; set; }

        /// <summary>
        /// لیست query string
        /// </summary>
        Dictionary<string, string[]> QueryStrings { get; set; }

        /// <summary>
        /// مقادیر ارسالی در body [fromBody]
        /// </summary>
        string Body { get; set; }

        /// <summary>
        /// زبان درخواستی
        /// </summary>
        string Lang { get; set; }

        /// <summary>
        /// آیا درخواست دارای اعتبارسنجی می باشد
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// اطلاعات توکن ارسالی
        /// </summary>
        UserTokenModel TokenInfo { get; set; }

    }
}