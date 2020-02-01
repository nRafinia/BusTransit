namespace Engine.Accounting.Models.Actions
{
    public class Actions:IAction
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ProviderName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Subject { get; set; }
        public bool IsActive { get; set; }
    }
}
