using System.Threading.Tasks;

namespace ANA.Common
{
    public abstract class ReadOnlyReceiver:BaseReceiver
    {
        protected abstract Task ProcessSendMessage(MessageDataModel request);


        protected ReadOnlyReceiver()
        {
            Bus.SubscribeAsync<MessageDataModel>(
                QueueName+"R",
                HandleSendMessage,
                c => c.WithQueueName(QueueName+"R"));

        }

        private async Task HandleSendMessage(MessageDataModel request)
        {
            await ProcessSendMessage(request);
        }

    }
}