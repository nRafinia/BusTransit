using System;
using System.Threading.Tasks;
using Common.Containers;
using Common.QMessageModels;
using Common.QMessageModels.RequestMessages;

namespace Common.Transmitters
{
    public interface ITransmitter : ITransient, IDisposable
    {
        /// <summary>
        /// ارسال درخواست و انتظار جهت دریافت پاسخ
        /// </summary>
        /// <param name="request">اطلاعات ارسالی</param>
        /// <param name="setting">اطلاعات صف</param>
        /// <returns>پاسخ دریافتی</returns>
        Task<QBaseResponse> Request(QSettingModel setting, QBaseRequest request);

        /// <summary>
        /// ارسال درخواست بدون پاسخ
        /// </summary>
        /// <param name="request">اطلاعات ارسالی</param>
        /// <param name="setting">اطلاعات صف</param>
        Task Send(QSettingModel setting, QBaseMessage request);
    }
}