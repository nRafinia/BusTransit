using System;

namespace Common.Models
{
    public class UserTokenModel
    {
        /// <summary>
        /// توکن
        /// </summary>
        public string TokenId { get; set; }

        /// <summary>
        /// کد کاربر
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// زمان ابطال
        /// </summary>
        public DateTime ExpireDate { get; set; }

        /// <summary>
        /// آیا موقت است
        /// </summary>
        public bool IsTemp { get; set; }
    }
}