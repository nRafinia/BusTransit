using System;
using System.Net.Http;
using Common;
using Common.CacheMemory;
using Common.Containers;
using Common.Mappers;
using Common.Models;
using Common.Tools;
using Common.Transmitters;
using Interface.Controllers;
using Interface.Models;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Engine.Accounting;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Interface.Tools
{
    public class UIInstaller : IIoCInstaller
    {
        public void Install(WindsorContainer container, IMapper mapper)
        {
            container.Register(Component.For<IInitProject>().ImplementedBy<InitProjectImp>().LifestyleSingleton());

            var appS = IoC.Resolve<IAppSetting>();
            var cConfig = appS.Get<ServiceConfigModel>("Accounting");

            var cService = RestService.For<IAuthenticateUser>(new HttpClient()
            {
                BaseAddress = new Uri(cConfig.Url),
                Timeout = TimeSpan.FromSeconds(cConfig.TimOut),
            });

            container.Register(Component.For<IAuthenticateUser>().Instance(cService).LifestyleSingleton());

        }
    }
}