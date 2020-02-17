using System;
using System.Net.Http;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Common.CacheMemory;
using Common.Models;
using F4ST.Common.Containers;
using F4ST.Common.Mappers;
using F4ST.Common.Tools;
using F4ST.Data;
using F4ST.Data.RavenDB;
using Refit;

namespace Common.Tools
{
    public class CommonInstaller : IIoCInstaller
    {
        public int Priority => 9;
        public void Install(WindsorContainer container, IMapper mapper)
        {
            container.Register(Component.For<IInterceptor>().ImplementedBy<CacheMethodInterceptor>());

            container.Register(Component.For<ICacheMemory>().ImplementedBy<CacheMem>().LifestyleSingleton());
            container.Register(Component.For<ICachingKeyGenerator>().ImplementedBy<CachingKeyGenerator>().LifestyleSingleton());

            var appSetting = IoC.Resolve<IAppSetting>();
            var cConfig = appSetting.Get<ServiceConfigModel>("CacheMemory");

            var cService = RestService.For<ICacheService>(new HttpClient()
            {
                BaseAddress = new Uri(cConfig.Url),
                Timeout = TimeSpan.FromSeconds(cConfig.TimOut),
            });

            container.Register(Component.For<ICacheService>().Instance(cService).LifestyleSingleton());

        }

        
    }
}