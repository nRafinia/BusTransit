using System.Globalization;
using Common.Containers;
using Common.QMessageModels.RequestMessages;
using Common.QMessageModels.SendMessages;
using Common.Tools;
using System.Reflection;
using System.Threading.Tasks;
using Common.QMessageModels;

namespace Common.Transmitters
{
    public class RPCTransmitter<T> : BaseProxy<T, RPCTransmitter<T>>
        where T : class
    {
        private QSettingModel qSetting;

        public static T Register(QSettingModel setting)
        {
            var res = CreateProxy(null);
            (res as RPCTransmitter<T>)?.SetQueueName(setting);

            return res;
        }

        private void SetQueueName(QSettingModel setting)
        {
            qSetting = setting;
        }

        protected override async Task<bool> BeforeRunMethod(MethodInfo targetMethod, object[] args)
        {
            await base.BeforeRunMethod(targetMethod, args);

            using (var transmitter = IoC.Resolve<ITransmitter>())
            {
                if (targetMethod.ReturnType == typeof(void))
                {
                    await transmitter.Send(qSetting, new QClassMessage()
                    {
                        Lang = CultureInfo.DefaultThreadCurrentCulture.Name,
                        MethodName = targetMethod.Name,
                        Parameters = args
                    });
                }
                else
                {
                    var res = await transmitter.Request(qSetting, new QClassRequestMessage()
                    {
                        Lang = CultureInfo.DefaultThreadCurrentCulture.Name,
                        MethodName = targetMethod.Name,
                        Parameters = args
                    });

                    if (res == null)
                    {
                        Result = null;
                        return false;
                    }

                    if (!(res is QClassResponseMessage response))
                    {
                        Result = null;
                        return false;
                    }

                    Result = response.Result;

                    if (!IsAsyncMethod(targetMethod))
                        return false;

                    if (targetMethod.ReturnType == typeof(Task))
                    {
                        Result = GetTaskResult();
                        return false;
                    }

                    var m = GetType().GetMethod("GetGenericResult");
                    var g = m?.MakeGenericMethod(targetMethod.ReturnType.GenericTypeArguments[0]);
                    Result = g?.Invoke(this, new[] {Result});
                }
            }

            return false;
        }

        public async Task<TT> GetGenericResult<TT>(TT result)
        {
            return result;
        }

        private async Task GetTaskResult()
        {
        }

    }
}