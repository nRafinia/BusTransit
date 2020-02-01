namespace Common.QMessageModels.SendMessages
{
    public class QClassMessage : QBaseMessage, IQClassMessage
    {
        public string Lang { get; set; }
        public string MethodName { get; set; }
        public object[] Parameters { get; set; }
        
    }
}