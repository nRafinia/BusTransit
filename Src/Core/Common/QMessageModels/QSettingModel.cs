namespace Common.QMessageModels
{
    public class QSettingModel
    {
        /// <summary>
        /// نام تنظیمات
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// پیشوند آدرس ها (URL)
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// نام صف
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// فقط پس از داشتن توکن دسترسی مجاز است
        /// </summary>
        public bool MustAuthorize { get; set; }

        /// <summary>
        /// نوع تنظیم
        /// </summary>
        public QSettingType SettingType { get; set; }
        
        /// <summary>
        /// آدرس سرور Queue
        /// </summary>
        public string ServerAddress { get; set; }

        /// <summary>
        /// نام کاربری Queue
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// کلمه عبور Queue
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// زمان انتظار جواب به ثانیه
        /// </summary>
        public int? Timeout { get; set; }

        /// <summary>
        /// آیا تنظیمات فعال می باشد
        /// </summary>
        public bool Active { get; set; }
    }
}