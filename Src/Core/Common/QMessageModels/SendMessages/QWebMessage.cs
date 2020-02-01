using System;
using System.Collections.Generic;
using System.Linq;
using Common.Models;

namespace Common.QMessageModels.SendMessages
{
    public class QWebMessage : QBaseMessage, IQWebMessage
    {
        /// <inheritdoc />
        public string TraceId { get; set; }

        /// <inheritdoc />
        public string Scheme { get; set; }

        /// <inheritdoc />
        public string Domain { get; set; }

        /// <inheritdoc />
        public string BasePath { get; set; }

        /// <inheritdoc />
        public string BaseUrl => $"{Scheme}://{Domain}{BasePath}/";

        public string Target => Url.Split('/').FirstOrDefault();

        /// <inheritdoc />
        public string Url { get; set; }

        /// <inheritdoc />
        public string IP { get; set; }

        /// <inheritdoc />
        public string HttpMethod { get; set; }

        /// <inheritdoc />
        public Dictionary<string, string[]> Headers { get; set; }

        /// <inheritdoc />
        public IDictionary<string, object> Arguments { get; set; }

        /// <inheritdoc />
        public Dictionary<string, string[]> QueryStrings { get; set; }

        /// <inheritdoc />
        public string Body { get; set; }

        /// <inheritdoc />
        public string Lang { get; set; }

        /// <inheritdoc />
        public bool IsAuthenticated => TokenInfo!=null;

        /// <inheritdoc />
        public UserTokenModel TokenInfo { get; set; }
    }
}