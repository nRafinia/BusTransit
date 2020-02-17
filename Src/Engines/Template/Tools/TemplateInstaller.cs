using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Containers;
using Common.Extensions;
using Common.QMessageModels;
using Common.Tools;
using Common.Transmitters;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using F4ST.Common.Containers;
using F4ST.Common.Mappers;
using F4ST.Common.Tools;
using F4ST.Queue.Extensions;
using F4ST.Queue.QMessageModels;
using F4ST.Queue.Transmitters;
using Template.Controllers;
using Template.Models;

namespace Template.Tools
{
    public class TemplateInstaller : IIoCInstaller
    {
        public int Priority => 10;
        public void Install(WindsorContainer container, IMapper mapper)
        {
            container.Register(Component.For<IInitProject>().ImplementedBy<InitProjectImp>().LifestyleSingleton());

            var appSetting = IoC.Resolve<IAppSetting>();
            var test = RPCTransmitter<ITestClass>.Register(appSetting.GetSetting("RPCTestClass"));
            container.Register(Component.For<ITestClass>().Instance(test).LifestyleSingleton());

            var qs = appSetting.Get<List<QSettingModel>>("QueueSettings");
            var item = qs.FirstOrDefault(q => q.Name == "WebTestClass");
            container.AddWebReceiverService<WebTestReceiver>(item);
            item = qs.FirstOrDefault(q => q.Name == "RPCTestClass");
            container.AddRpcReceiverService<RpcTestReceiver>(item);

            mapper.Bind<SourceModel, DestModel>()
                .Map(d => d.RandNum, s => new Random().Next(1000));

            var src = new SourceModel()
            {
                Name = "Hello",
                Id = 100
            };

            var des = src.MapTo<DestModel>();


        }
    }
}