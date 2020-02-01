namespace Common.Models
{
    public class ServiceConfigModel
    {
        /// <summary>
        /// آدرس سرویس کش
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// زمان انتظار به ثانیه
        /// </summary>
        public int TimOut { get; set; }
    }
}