using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Containers;
using Common.QMessageModels;
using Common.Tools;
using F4ST.Common.Containers;
using F4ST.Common.Tools;
using F4ST.Queue.QMessageModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace Interface.Tools
{
    public class LandingPageTransformer : DynamicRouteValueTransformer, ITransient
    {
        private readonly IAppSetting _appSetting;

        public LandingPageTransformer(IAppSetting appSetting)
        {
            _appSetting = appSetting;
        }

        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            var requestPath = httpContext.Request.Path.Value.ToLower();
            if (!string.IsNullOrEmpty(requestPath) && requestPath[0] == '/')
            {
                requestPath = requestPath.Substring(1);
            }

            var qSettings = _appSetting.Get<List<QSettingModel>>("QueueSettings");
            var mItems = qSettings?.FindAll(s => requestPath.StartsWith(s.Prefix.ToLower()));
            var item = mItems?.OrderByDescending(s => s.Prefix.Length).FirstOrDefault();

            var pageFound = item != null;
            if (!pageFound)
                return values;


            //TODO: Handle query strings
            values["controller"] = "Api";
            values["action"] = "Index";

            return values;
        }
    }
}