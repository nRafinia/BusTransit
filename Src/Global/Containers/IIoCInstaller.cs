using System;
using Common.Mappers;
using Castle.Windsor;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Containers
{
    public interface IIoCInstaller
    {
        void Install(WindsorContainer container, IMapper mapper);
    }
}