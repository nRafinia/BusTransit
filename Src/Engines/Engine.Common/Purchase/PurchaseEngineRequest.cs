namespace Engine.Common.Purchase
{
    public class PurchaseEngineRequest
    {
        /// <summary>
        /// کد منحصر بفرد خرید
        /// </summary>
        public string PurchaseId { get; set; }

        /// <summary>
        /// نوع محصول
        /// </summary>
        public string ProductId { get; set; }
        
        /// <summary>
        /// تعداد
        /// </summary>
        public decimal? Quantity { get; set; }
        
        /// <summary>
        /// کد کاربر
        /// </summary>
        public string UserId { get; set; }  
        
        /// <summary>
        /// کانال فروش
        /// </summary>
        public string SaleChannel { get; set; }
        
        /// <summary>
        /// جزئیات خرید که به صورت ساختار
        /// json می باشد
        /// </summary>
        public string PurchaseDetails { get; set; }
    }
}