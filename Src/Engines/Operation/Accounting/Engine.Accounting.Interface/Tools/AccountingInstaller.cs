using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Containers;
using Common.Extensions;
using Engine.Accounting.Controllers;
using Engine.Accounting.Data;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using F4ST.Common.Containers;
using F4ST.Common.Mappers;
using F4ST.Common.Tools;
using F4ST.Queue.Extensions;
using F4ST.Queue.QMessageModels;

namespace Engine.Accounting.Tools
{
    public class AccountingInstaller:IIoCInstaller
    {
        public int Priority => 11;
        public void Install(WindsorContainer container, IMapper mapper)
        {
            container.Register(Component.For<IInitProject>().ImplementedBy<InitProjectImp>().LifestyleSingleton());

            /*mapper.Bind<UserInfo, BaseUserInfo>();
            mapper.Bind<UsersEntity, UserInfo>();*/

            var appSetting = IoC.Resolve<IAppSetting>();
            var qs=appSetting.Get<List<QSettingModel>>("QueueSettings");
            var item = qs.FirstOrDefault(q => q.Name == "AccountingWeb");
            container.AddWebReceiverService<AccountingWebReceiver>(item);
            //container.AddRpcReceiverService<AccountingRpcReceiver>("AccountingRPC");
        }
    }
}