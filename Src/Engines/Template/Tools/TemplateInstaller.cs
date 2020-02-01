using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Containers;
using Common.Extensions;
using Common.Mappers;
using Common.QMessageModels;
using Common.Tools;
using Common.Transmitters;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Template.Controllers;
using Template.Models;

namespace Template.Tools
{
    public class TemplateInstaller : IIoCInstaller
    {
        public void Install(WindsorContainer container, IMapper mapper)
        {
            container.Register(Component.For<IInitProject>().ImplementedBy<InitProjectImp>().LifestyleSingleton());

            var appSetting = IoC.Resolve<IAppSetting>();
            var test = RPCTransmitter<ITestClass>.Register(appSetting.GetSetting("RPCTestClass"));
            container.Register(Component.For<ITestClass>().Instance(test).LifestyleSingleton());

            container.AddWebReceiverService<WebTestReceiver>("WebTestClass");
            container.AddRpcReceiverService<RpcTestReceiver>("RPCTestClass");

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