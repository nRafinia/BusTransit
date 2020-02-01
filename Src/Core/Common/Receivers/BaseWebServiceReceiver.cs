using Common.Attributes;
using Common.Extensions;
using Common.QMessageModels.RequestMessages;
using Common.Tools;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Common.Containers;
using Common.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Common.Receivers
{
    public class BaseWebServiceReceiver<TReceiver> : Receiver<QWebRequestMessage, QWebResponse, QWebRequestMessage>
        where TReceiver : WebServiceReceiver
    {

        protected override bool HaveRequestMessage => true;
        protected override bool HaveSendMessage => false;

        //protected int? ReturnStatus = null;
        //protected Dictionary<string, string> ReturnHeader = null;

        public BaseWebServiceReceiver(string settingName) : base(settingName)
        {
        }

        public static string GetCurrentNamespace()
        {
            return new StackFrame(2)?.GetMethod().DeclaringType?.Namespace ?? "";
        }

        private (int, string, MethodInfo, List<object>, string) FindMethod(TReceiver rcInstance, QWebRequestMessage request)
        {
            (int, string, MethodInfo, List<object>, string) res;
            try
            {
                var path = request.Url.ToLower().Split('/').Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

                if (path.Length <= 1)
                {
                    return ((int)GlobalStatusCode.NotFound, GlobalStatusCode.NotFound.GetEnumName()+", 002", null, null,
                        GlobalStatusCode.NotFound.GetEnumName()+", 002");
                }

                var tMethod = path[1];
                var methods = rcInstance.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
                var cMethods = methods.Where(m => m.Name.ToLower() == tMethod).ToArray();
                var cMethod = cMethods.FirstOrDefault();

                if (cMethods.Length > 1)
                {
                    Type attr;
                    switch (request.HttpMethod.ToUpper())
                    {
                        case "GET":
                            attr = typeof(QGetAttribute);
                            break;
                        case "POST":
                            attr = typeof(QPostAttribute);
                            break;
                        case "PUT":
                            attr = typeof(QPutAttribute);
                            break;
                        case "PATCH":
                            attr = typeof(QPatchAttribute);
                            break;
                        case "DELETE":
                            attr = typeof(QDeleteAttribute);
                            break;
                        case "HEAD":
                            attr = typeof(QHeadAttribute);
                            break;
                        case "OPTIONS":
                            attr = typeof(QOptionsAttribute);
                            break;
                        default:
                            attr = null;
                            break;
                    }

                    MethodInfo me = null;
                    if (attr != null)
                    {
                        me = cMethods.FirstOrDefault(m => m.GetCustomAttribute(attr) != null);
                    }

                    if (me == null)
                    {
                        me = cMethods.FirstOrDefault(m => m.GetCustomAttribute<QHttpMethodAttribute>(true) == null);
                    }

                    if (me != null)
                    {
                        cMethod = me;
                    }
                }

                if (cMethod == null)
                {
                    return ((int)GlobalStatusCode.NotFound, GlobalStatusCode.NotFound.GetEnumName()+", 003", null, null,
                        GlobalStatusCode.NotFound.GetEnumName()+", 003");
                }

                List<object> mParams = null;
                var cParam = cMethod.GetParameters();
                if (cParam.Any())
                {
                    mParams = new List<object>();

                    var route = cMethod.GetCustomAttribute<QRouteAttribute>()?.Route?.ToLower()
                        .Split('/')
                        .Where(p => !string.IsNullOrWhiteSpace(p))
                        .ToArray();

                    foreach (var prm in cParam)
                    {
                        var prmName = prm.Name.ToLower();

                        object p = null;

                        var qfb = prm.GetCustomAttribute<QFromBodyAttribute>();
                        if (qfb != null)
                        {
                            p = JsonConvert.DeserializeObject(request.Body, prm.ParameterType);
                        }

                        var qfh = prm.GetCustomAttribute<QFromHeaderAttribute>();
                        if (p == null && qfh != null && request.Headers.ContainsKey(prmName))
                        {
                            var v = request.Headers[prmName];
                            p = v.ConvertToType<string>(prm.ParameterType);
                        }

                        if (p == null && request.QueryStrings.ContainsKey(prmName))
                        {
                            var v = request.QueryStrings[prmName];
                            p = v.ConvertToType<string>(prm.ParameterType);
                        }

                        if (p == null && (route?.Any() ?? false))
                        {
                            var rf = Array.IndexOf(route, prmName);
                            if (rf >= 0 && path.Length > rf + 2)
                            {
                                p = path[rf + 2].ConvertToType<string>(prm.ParameterType);
                            }
                        }

                        if (p == null && prm.HasDefaultValue)
                        {
                            p = prm.DefaultValue;
                        }

                        mParams.Add(p);
                    }
                }

                res = ((int)GlobalStatusCode.Ok, "", cMethod, mParams, null);
            }
            catch (Exception e)
            {
//#if DEBUG
                res = ((int)GlobalStatusCode.InternalServerError, GlobalStatusCode.InternalServerError.GetEnumName(),
                    null, null, JsonConvert.SerializeObject(e));
/*#else
                res =
 ((int)GlobalStatusCode.InternalServerError, GlobalStatusCode.InternalServerError.GetEnumName(), null, null, 
                    GlobalStatusCode.InternalServerError.GetEnumName());
#endif*/
            }

            return res;
        }

        protected override async Task<QWebResponse> ProcessRequestMessage(QWebRequestMessage request)
        {
            QWebResponse res;
            try
            {
                //var wr = Activator.CreateInstance<TReceiver>();
                var wr = IoC.Resolve<TReceiver>();
                wr.Request = request;

                var fm = FindMethod(wr, request);

                if (fm.Item1 != (int)GlobalStatusCode.Ok)
                {
                    return new QWebResponse()
                    {
                        TraceId = request.TraceId,
                        Status = fm.Item1,
                        Response = JsonConvert.SerializeObject(fm.Item5)
                    };
                }

                var authM = fm.Item3.GetCustomAttribute<QAuthAttribute>(false);
                var authC = fm.Item3.GetCustomAttribute<QAuthAttribute>(true);
                var anonymousM = fm.Item3.GetCustomAttribute<QAllowAnonymousAttribute>(false);
                //var anonymousC = fm.Item2.GetCustomAttribute<QAllowAnonymousAttribute>(true);
                if ((authM != null && !request.IsAuthenticated) ||
                    (authC != null && anonymousM == null && !request.IsAuthenticated))
                {
                    return new QWebResponse()
                    {
                        TraceId = request.TraceId,
                        Status = (int)GlobalStatusCode.AccessDenied,
                        Response = JsonConvert.SerializeObject(GlobalStatusCode.AccessDenied.GetEnumName())
                    };
                }

                if (request.IsAuthenticated)
                {

                }

                if (!string.IsNullOrWhiteSpace(request.Lang))
                {
                    var cultureInfo = new CultureInfo(request.Lang);
                    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                    CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
                }

                var methodRes = Globals.RunMethod(wr, fm.Item3, fm.Item4?.ToArray(), true);

                var statusCode = (int)GlobalStatusCode.Ok;
                if (methodRes is BaseResponse mRes && mRes.Status != (int)GlobalStatusCode.Ok)
                {
                    var stCode = EnumExt.GetEnumFromInt<GlobalStatusCode>(mRes.Status);
                    if (stCode != (GlobalStatusCode)0)
                    {
                        statusCode = (int)stCode;
                    }
                }

                res = new QWebResponse
                {
                    Response = JsonSerializer.Serialize(methodRes),
                    Status = statusCode,
                    TraceId = request.TraceId,
                    Headers = wr.ResponseHeader
                };
            }
            catch (Exception e)
            {
#if DEBUG
                res = new QWebResponse()
                {
                    TraceId = request.TraceId,
                    Status = (int)GlobalStatusCode.NotFound,
                    Response = JsonConvert.SerializeObject(e)
                };
#else
                res = new QWebResponse()
                {
                    TraceId = request.TraceId,
                    Status = (int)GlobalStatusCode.NotFound,
                    Response = JsonConvert.SerializeObject(GlobalStatusCode.NotFound.GetEnumName())
                };
#endif
            }

            return res;
        }

        protected override async Task ProcessSendMessage(QWebRequestMessage request)
        {
            try
            {
                //var wr = Activator.CreateInstance<TReceiver>();
                var wr = IoC.Resolve<TReceiver>();
                wr.Request = request;

                var fm = FindMethod(wr, request);

                if (fm.Item1 != (int)GlobalStatusCode.Ok)
                {
                    return;
                }

                var authM = fm.Item3.GetCustomAttribute<QAuthAttribute>(false);
                var authC = fm.Item3.GetCustomAttribute<QAuthAttribute>(true);
                var anonymousM = fm.Item3.GetCustomAttribute<QAllowAnonymousAttribute>(false);
                //var anonymousC = fm.Item2.GetCustomAttribute<QAllowAnonymousAttribute>(true);
                if ((authM != null && !request.IsAuthenticated) ||
                    (authC != null && anonymousM == null && !request.IsAuthenticated))
                {
                    return;
                }

                if (!string.IsNullOrWhiteSpace(request.Lang))
                {
                    var cultureInfo = new CultureInfo(request.Lang);
                    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                    CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
                }

                await (Task<QWebResponse>)fm.Item3.Invoke(wr, parameters: fm.Item4?.ToArray());
            }
            catch //(Exception e)
            {
                //
            }
        }

        public void Start()
        {
        }

    }
}