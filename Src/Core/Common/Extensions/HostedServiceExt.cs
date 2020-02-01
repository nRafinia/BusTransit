using System;
using Common.Receivers;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Common.Extensions
{
    public static class HostedServiceExt
    {
        public static void AddWebReceiverService<THostedService>(this WindsorContainer container, string settingName)
            where THostedService : WebServiceReceiver
        {
            var wr = new BaseWebServiceReceiver<THostedService>(settingName);
            wr.Start();

            container.Register(Component.For<THostedService>());
            
        }

        public static void AddRpcReceiverService<THostedService>(this WindsorContainer container, string settingName)
            where THostedService : RPCReceiver
        {
            var wr = new BaseRPCReceiver<THostedService>(settingName);
            wr.Start();

            container.Register(Component.For<THostedService>());
            
        }
    }
}