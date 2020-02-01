using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Containers;
using Common.Extensions;
using Common.QMessageModels;
using Common.QMessageModels.RequestMessages;
using Common.Tools;
using EasyNetQ;

namespace Common.Receivers
{
    public abstract class Receiver<TRequest, TResponse, TMessage> : IDisposable
        where TRequest : QBaseRequest
        where TResponse : QBaseResponse
        where TMessage : QBaseMessage
    {
        protected abstract Task<TResponse> ProcessRequestMessage(TRequest request);
        protected abstract Task ProcessSendMessage(TMessage request);

        protected abstract bool HaveRequestMessage { get; }
        protected abstract bool HaveSendMessage { get; }

        private IBus _bus;

        protected Receiver(string settingName)
        {
            var appSetting = IoC.Resolve<IAppSetting>();
            var qSetting = appSetting.GetSetting(settingName);

            var connectionString = QCreateConnectionString.CreateConnection(qSetting);
            _bus = RabbitHutch.CreateBus(connectionString);

            if (HaveRequestMessage)
            {
                _bus.RespondAsync<TRequest, TResponse>(HandleRequestMessage,
                    c => c.WithQueueName(qSetting.QueueName));
            }

            if (HaveSendMessage)
            {
                _bus.SubscribeAsync<TMessage>(
                    $"{qSetting.QueueName}_R",
                    HandleSendMessage,
                    c => c.WithQueueName($"{qSetting.QueueName}_R"));
            }
        }

        private async Task<TResponse> HandleRequestMessage(TRequest request)
        {
            var res = await ProcessRequestMessage(request);
            return res;
        }

        private async Task HandleSendMessage(TMessage request)
        {
            await ProcessSendMessage(request);
        }

        public void Dispose()
        {
            _bus?.Dispose();
            _bus = null;
        }
    }

    public abstract class Receiver : Receiver<QBaseRequest, QBaseResponse, QBaseMessage>
    {
        protected Receiver(string settingName) : base(settingName)
        {

        }
    }
}