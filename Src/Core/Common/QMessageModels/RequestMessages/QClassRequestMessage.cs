namespace Common.QMessageModels.RequestMessages
{
    public class QClassRequestMessage:QBaseRequest, IQClassMessage
    {
        public string Lang { get; set; }
        public string MethodName { get; set; }
        public object[] Parameters { get; set; }

    }
}