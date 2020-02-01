using Common;
using Common.Containers;
using Common.Extensions;
using Common.Mappers;
using Engine.Accounting.Controllers;
using Engine.Accounting.Data;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Engine.Accounting.Tools
{
    public class AccountingInstaller:IIoCInstaller
    {
        public void Install(WindsorContainer container, IMapper mapper)
        {
            container.Register(Component.For<IInitProject>().ImplementedBy<InitProjectImp>().LifestyleSingleton());

            /*mapper.Bind<UserInfo, BaseUserInfo>();
            mapper.Bind<UsersEntity, UserInfo>();*/

            container.AddWebReceiverService<AccountingWebReceiver>("AccountingWeb");
            container.AddRpcReceiverService<AccountingRpcReceiver>("AccountingRPC");
        }
    }
}