using ANA.Common.CacheMemory;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Net.Http;
using ANA.Common.Mappers;
using ANA.Common.Models;
using ANA.Common.Tools;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace ANA.Common.Containers
{
    public class CommonInstaller : IIoCInstaller
    {
        public void Install(WindsorContainer container, IMapper mapper)
        {
            container.Register(Component.For<ICacheMemory>().ImplementedBy<CacheMem>().LifestyleSingleton());
            container.Register(Component.For<ICachingKeyGenerator>().ImplementedBy<CachingKeyGenerator>().LifestyleSingleton());
            container.Register(Component.For<IAppSetting>().ImplementedBy<AppSetting>().LifestyleSingleton());

            var appS = IoC.Resolve<IAppSetting>();
            var cConfig = appS.Get<ServiceConfigModel>("CacheMemory");

            var cService = RestService.For<ICacheService>(new HttpClient()
            {
                BaseAddress = new Uri(cConfig.Url),
                Timeout = TimeSpan.FromSeconds(cConfig.TimOut),
            });

            container.Register(Component.For<ICacheService>().Instance(cService).LifestyleSingleton());
        }
    }
}