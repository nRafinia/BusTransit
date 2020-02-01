using System.Collections.Generic;
using Common.QMessageModels.RequestMessages;

namespace Common.Receivers
{
    public abstract class WebServiceReceiver
    {
        public QWebRequestMessage Request { get; set; }
        public Dictionary<string, string> ResponseHeader = null;
    }
}