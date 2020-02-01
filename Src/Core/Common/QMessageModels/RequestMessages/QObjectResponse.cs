namespace Common.QMessageModels.RequestMessages
{
    public class QObjectResponse:QBaseResponse
    {
        /// <summary>
        /// پاسخ ارسالی
        /// </summary>
        public object Response { get; set; }
    }
}