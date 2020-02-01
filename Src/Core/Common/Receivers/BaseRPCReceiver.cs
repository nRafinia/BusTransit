using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Common.Attributes;
using Common.Containers;
using Common.QMessageModels;
using Common.QMessageModels.RequestMessages;
using Common.QMessageModels.SendMessages;
using Common.Tools;
using Microsoft.Extensions.Hosting;

namespace Common.Receivers
{
    public class BaseRPCReceiver<TReceiver> : Receiver<QClassRequestMessage, QClassResponseMessage, QClassMessage>
        where TReceiver : RPCReceiver
    {
        //protected override string QueueName => GetQueueAttributeName() ?? GetType().Name;
        protected override bool HaveRequestMessage => true;
        protected override bool HaveSendMessage => true;


        public BaseRPCReceiver(string settingName) : base(settingName)
        {
        }

        public static string GetCurrentNamespace()
        {
            return new StackFrame(2)?.GetMethod().DeclaringType?.Namespace ?? "";
        }

        private MethodInfo FindMethod(TReceiver rcInstance, string targetMethod)
        {
            var methods = rcInstance.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var cMethods = methods.Where(m => m.Name == targetMethod).ToArray();
            return cMethods.FirstOrDefault();
        }

        protected override async Task<QClassResponseMessage> ProcessRequestMessage(QClassRequestMessage request)
        {
            var wr = IoC.Resolve<TReceiver>();
            var method = FindMethod(wr, request.MethodName);

            if (!string.IsNullOrWhiteSpace(request.Lang))
            {
                var cultureInfo = new CultureInfo(request.Lang);
                CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            }

            var res = Globals.RunMethod(wr, method, request.Parameters, true);
            return new QClassResponseMessage()
            {
                Result = res
            };
        }

        protected override async Task ProcessSendMessage(QClassMessage request)
        {
            var wr = IoC.Resolve<TReceiver>();
            var method = FindMethod(wr, request.MethodName);

            if (!string.IsNullOrWhiteSpace(request.Lang))
            {
                var cultureInfo = new CultureInfo(request.Lang);
                CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            }

            Globals.RunMethod(wr, method, request.Parameters, false);
        }

        public void Start()
        {
        }

    }
}