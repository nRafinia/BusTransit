using System;
using System.Net.Http;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Common.CacheMemory;
using Common.Containers;
using Common.Data;
using Common.Data.Dapper;
using Common.Data.LiteDB;
using Common.Data.RavenDb;
using Common.Mappers;
using Common.Models;
using Refit;

namespace Common.Tools
{
    public class CommonInstaller : IIoCInstaller
    {
        public void Install(WindsorContainer container, IMapper mapper)
        {
            container.Register(Component.For<IInterceptor>().ImplementedBy<CacheMethodInterceptor>());

            container.Register(Component.For<ICacheMemory>().ImplementedBy<CacheMem>().LifestyleSingleton());
            container.Register(Component.For<ICachingKeyGenerator>().ImplementedBy<CachingKeyGenerator>().LifestyleSingleton());
            container.Register(Component.For<IAppSetting>().ImplementedBy<AppSetting>().LifestyleSingleton());

            var appSetting = IoC.Resolve<IAppSetting>();
            var cConfig = appSetting.Get<ServiceConfigModel>("CacheMemory");

            var cService = RestService.For<ICacheService>(new HttpClient()
            {
                BaseAddress = new Uri(cConfig.Url),
                Timeout = TimeSpan.FromSeconds(cConfig.TimOut),
            });

            container.Register(Component.For<ICacheService>().Instance(cService).LifestyleSingleton());

            SetDbConnection(container);
        }

        /// <summary>
        /// Find and set database repository handler
        /// </summary>
        /// <param name="container">Windsor container</param>
        private static void SetDbConnection(WindsorContainer container)
        {
            var appSetting = IoC.Resolve<IAppSetting>();
            var config = appSetting.Get<DbConnectionModel>("DbConnection");

            switch (config.DatabaseType)
            {
                case DatabaseType.RavenDB:
                    config = appSetting.Get<RavenDbConnectionConfig>("DbConnection",true); 
                    container.Register(Component.For<IRepository>().ImplementedBy<RavenDbRepository>().LifestyleTransient());
                    break;
                
                case DatabaseType.SQLServer:
                    config = appSetting.Get<DapperConnectionConfig>("DbConnection",true); 
                    DapperFramework.SetDialect(DapperFramework.Dialect.SQLServer);
                    container.Register(Component.For<IRepository>().ImplementedBy<DapperRepository>().LifestyleTransient());
                    break;
                case DatabaseType.PostgreSQL:
                    config = appSetting.Get<DapperConnectionConfig>("DbConnection",true); 
                    DapperFramework.SetDialect(DapperFramework.Dialect.PostgreSQL);
                    container.Register(Component.For<IRepository>().ImplementedBy<DapperRepository>().LifestyleTransient());
                    break;
                case DatabaseType.SQLite:
                    config = appSetting.Get<DapperConnectionConfig>("DbConnection",true); 
                    DapperFramework.SetDialect(DapperFramework.Dialect.SQLite);
                    container.Register(Component.For<IRepository>().ImplementedBy<DapperRepository>().LifestyleTransient());
                    break;
                case DatabaseType.MySQL:
                    config = appSetting.Get<DapperConnectionConfig>("DbConnection",true); 
                    DapperFramework.SetDialect(DapperFramework.Dialect.MySQL);
                    container.Register(Component.For<IRepository>().ImplementedBy<DapperRepository>().LifestyleTransient());
                    break;

                case DatabaseType.LiteDB:
                    config = appSetting.Get<LiteDbConnectionConfig>("DbConnection",true); 
                    container.Register(Component.For<IRepository>().ImplementedBy<LiteDbRepository>().LifestyleTransient());
                    break;
            }

            container.Register(Component.For<DbConnectionModel>().Instance(config).LifestyleSingleton());
        }
    }
}