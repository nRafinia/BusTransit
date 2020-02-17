using System;
using Common.Data;
using F4ST.Data;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Engine.Accounting.Data
{
    public class UserToken : DbEntity
    {
        public string UserId { get; set; }

        /// <summary>
        /// زمان باطل شدن توکن
        /// </summary>
        public DateTime ExpireDate { get; set; }

        /// <summary>
        /// توکن باطل شده است یا خیر
        /// </summary>
        public bool Expired { get; set; } = false;

        /// <summary>
        /// اطلاعات کاربر مربوطه
        /// </summary>
        [JsonIgnore]
        public User User { get; set; }
    }
}