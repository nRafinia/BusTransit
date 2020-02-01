namespace Engine.Accounting.Models.Actions
{
    public interface  IAction
    {
        /// <summary>
        /// کد
        /// </summary>
         string Id { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
         string Title { get; set; }
        /// <summary>
        /// نام Provider
        /// </summary>
         string ProviderName { get; set; }
        /// <summary>
        /// نام کنترلر
        /// </summary>
         string ControllerName { get; set; }
        /// <summary>
        /// نام اکشن
        /// </summary>
         string ActionName { get; set; }
        /// <summary>
        /// موضوع
        /// </summary>
         string Subject { get; set; }
        /// <summary>
        /// فعال/غیرفعال
        /// </summary>
         bool IsActive { get; set; }
    }
}
