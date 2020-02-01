using System;
using ANA.Common.Mappers;
using Castle.Windsor;
using Microsoft.Extensions.DependencyInjection;

namespace ANA.Common.Containers
{
    public interface IIoCInstaller
    {
        void Install(WindsorContainer container, IMapper mapper);
    }
}