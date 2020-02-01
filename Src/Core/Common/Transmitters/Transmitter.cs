using EasyNetQ;
using System;
using System.Text;
using System.Threading.Tasks;
using Common.QMessageModels;
using Common.QMessageModels.RequestMessages;
using EasyNetQ.Producer;
using EasyNetQ.Topology;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common.Transmitters
{
    public class Transmitter : ITransmitter
    {
        private IBus _bus;

        private void CreateBus(QSettingModel setting)
        {
            var connectionString = QCreateConnectionString.CreateConnection(setting);
            _bus = RabbitHutch.CreateBus(connectionString);
        }

        public async Task<QBaseResponse> Request(QSettingModel setting, QBaseRequest request)
        {
            CreateBus(setting);

            return await _bus.RequestAsync<QBaseRequest, QBaseResponse>(request,
                c => c.WithQueueName(setting.QueueName));
        }

        public async Task Send(QSettingModel setting, QBaseMessage request)
        {
            CreateBus(setting);
            await _bus.SendAsync(setting.QueueName + "_R", request);
        }

        public void Dispose()
        {
            _bus.Dispose();
            _bus = null;
        }
    }
}