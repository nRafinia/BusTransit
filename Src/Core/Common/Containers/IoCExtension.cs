﻿using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using F4ST.Common.Containers;
using Microsoft.Extensions.Hosting;

namespace Common.Containers
{
    public static class IoCExtension
    {
        public static IHostBuilder InitIoc(this IHostBuilder host) =>
            host
                .UseServiceProviderFactory(new IoC())
                .ConfigureContainer<WindsorContainer>((hostBuilderContext, windsorContainer) =>
                {
                    windsorContainer.Kernel.Resolver.AddSubResolver(new CollectionResolver(windsorContainer.Kernel,
                        true));
                    windsorContainer.AddFacility<TypedFactoryFacility>();

                    IoC.Container = windsorContainer;
                });
    }
}