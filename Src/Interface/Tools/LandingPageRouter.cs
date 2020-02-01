using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Containers;
using Common.QMessageModels;
using Common.Tools;
using Microsoft.AspNetCore.Routing;

namespace Interface.Tools
{
    public class LandingPageRouter : IRouter
    {
        private readonly IRouter _router;
        private readonly IAppSetting _appSetting;

        public LandingPageRouter(IRouter router)
        {
            _router = router;

            _appSetting = IoC.Resolve<IAppSetting>();
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }

        public Task RouteAsync(RouteContext context)
        {
            var requestPath = context.HttpContext.Request.Path.Value.ToLower();
            if (!string.IsNullOrEmpty(requestPath) && requestPath[0] == '/')
            {
                requestPath = requestPath.Substring(1);
            }

            var qSettings = _appSetting.Get<List<QSettingModel>>("QueueSettings");
            var mItems = qSettings?.FindAll(s => requestPath.StartsWith(s.Prefix.ToLower()));
            var item = mItems?.OrderByDescending(s => s.Prefix.Length).FirstOrDefault();

            var pageFound = item != null;
            if (!pageFound)
                return Task.FromResult(0);


            //TODO: Handle query strings
            var routeData = new RouteData();
            routeData.Values["controller"] = "Api";
            routeData.Values["action"] = "Index";
            context.RouteData = routeData;
            return _router.RouteAsync(context);
        }
    }
}