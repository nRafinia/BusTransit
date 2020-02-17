namespace Common.Authenticates
{
    public interface IBaseUserInfo
    {
        /// <summary>
        /// کد کاربر
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// نام کاربر
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// ایمیل
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// نام
        /// </summary>
        string Firstname { get; set; }

        /// <summary>
        /// نام خانوادگی
        /// </summary>
        string LastName { get; set; }

        /// <summary>
        /// وضعیت کاربر
        /// </summary>
        UserStatus Status { get; set; }
    }
}