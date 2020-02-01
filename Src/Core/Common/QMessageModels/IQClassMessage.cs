namespace Common.QMessageModels
{
    public interface IQClassMessage
    {
        string Lang { get; set; }
        string MethodName { get; set; }
        object[] Parameters { get; set; }
    }
}