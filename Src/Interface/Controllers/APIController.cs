using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Authenticates;
using Common.CacheMemory;
using Common.Extensions;
using Common.Models;
using Common.QMessageModels;
using Common.Tools;
using Common.Transmitters;
using Engine.Accounting;
using F4ST.Common.Tools;
using F4ST.Queue.QMessageModels;
using F4ST.Queue.QMessageModels.RequestMessages;
using F4ST.Queue.Transmitters;
using Newtonsoft.Json;

namespace Interface.Controllers
{
    public class ApiController : Controller
    {
        private QWebRequestMessage _sendingModel;

        private readonly ITransmitter _transmitter;
        private readonly IAppSetting _appSetting;
        private readonly IAuthenticateUtil _authenticateUtil;
        private readonly ICacheMemory _cacheMemory;
        private readonly IAuthenticateUser _authenticateService;

        public ApiController(ITransmitter transmitter, IAppSetting appSetting, IAuthenticateUtil authenticateUtil,
            ICacheMemory cacheMemory, IAuthenticateUser authenticateService)
        {
            _transmitter = transmitter;
            _appSetting = appSetting;
            _authenticateUtil = authenticateUtil;
            _cacheMemory = cacheMemory;
            _authenticateService = authenticateService;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var headers = Request?.Headers?.ToList()
                .ToDictionary(k => k.Key.ToLower(), v => v.Value.ToArray());

            var path = context.HttpContext.Request.Path.Value.Substring(1);
            var args = context.ActionArguments;
            var method = context.HttpContext.Request.Method;
            var scheme = context.HttpContext.Request.Scheme;
            var host = context.HttpContext.Request.Host.Value;
            var basePath = Url.Content("~");
            var queryS = context.HttpContext.Request.Query.ToDictionary(k => k.Key, v => v.Value.ToArray());
            var ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
            var lang = headers?.ContainsKey("lang") ?? false ? headers["lang"].FirstOrDefault() : string.Empty;
            var token = headers?.ContainsKey("key") ?? false ? headers["key"].FirstOrDefault() : string.Empty;

            UserTokenModel tokenInfo = null;
            if (!string.IsNullOrWhiteSpace(token))
            {
                tokenInfo = _authenticateUtil.ValidateToken(token);
            }

            var bodyStr = string.Empty;

            if (context.HttpContext.Request.ContentLength != null)
            {
                //context.HttpContext.AllowSynchronousIO
                using var reader = new StreamReader(context.HttpContext.Request.Body,
                    Encoding.UTF8, true, 1024, true);

                bodyStr =await reader.ReadToEndAsync();
            }

            _sendingModel = new QWebRequestMessage()
            {
                TraceId = context.HttpContext.TraceIdentifier,
                Scheme = scheme,
                Domain = host,
                BasePath = basePath,
                Url = path,
                IP = ip,
                HttpMethod = method,
                Headers = headers,
                Arguments = args,
                QueryStrings = queryS,
                Body = bodyStr,
                Lang = lang,
                //Token = token,
                //TokenInfo = tokenInfo
            };
            await base.OnActionExecutionAsync(context, next);

        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            return;
            base.OnActionExecuting(context);

            var headers = Request?.Headers?.ToList()
                .ToDictionary(k => k.Key.ToLower(), v => v.Value.ToArray());

            var path = context.HttpContext.Request.Path.Value.Substring(1);
            var args = context.ActionArguments;
            var method = context.HttpContext.Request.Method;
            var scheme = context.HttpContext.Request.Scheme;
            var host = context.HttpContext.Request.Host.Value;
            var basePath = Url.Content("~");
            var queryS = context.HttpContext.Request.Query.ToDictionary(k => k.Key, v => v.Value.ToArray());
            var ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
            var lang = headers?.ContainsKey("lang") ?? false ? headers["lang"].FirstOrDefault() : string.Empty;
            var token = headers?.ContainsKey("key") ?? false ? headers["key"].FirstOrDefault() : string.Empty;

            UserTokenModel tokenInfo = null;
            if (!string.IsNullOrWhiteSpace(token))
            {
                tokenInfo = _authenticateUtil.ValidateToken(token);
            }

            var bodyStr = string.Empty;

            if (context.HttpContext.Request.ContentLength != null)
            {
                //context.HttpContext.AllowSynchronousIO
                using var reader = new StreamReader(context.HttpContext.Request.Body, 
                    Encoding.UTF8, true, 1024, true);
                
                bodyStr = reader.ReadToEnd();
            }

            _sendingModel = new QWebRequestMessage()
            {
                TraceId = context.HttpContext.TraceIdentifier,
                Scheme = scheme,
                Domain = host,
                BasePath = basePath,
                Url = path,
                IP = ip,
                HttpMethod = method,
                Headers = headers,
                Arguments = args,
                QueryStrings = queryS,
                Body = bodyStr,
                Lang = lang,
                //Token = token,
                //TokenInfo = tokenInfo
            };
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var path = _sendingModel.Url.ToLower();
                var qSettings = _appSetting.Get<List<QSettingModel>>("QueueSettings");
                var mItems = qSettings?.FindAll(s => s.Active && path.StartsWith(s.Prefix.ToLower()));
                var queue = mItems?.OrderByDescending(s => s.Prefix.Length).FirstOrDefault();

                if (queue == null)
                {
                    return StatusCode((int) GlobalStatusCode.NotFound, GlobalStatusCode.NotFound.GetEnumName()+", 001");
                }

                if (queue.MustAuthorize)
                {
                    if (!_sendingModel.IsAuthenticated)
                    {
                        return StatusCode((int) GlobalStatusCode.AccessDenied,
                            GlobalStatusCode.AccessDenied.GetEnumName());
                    }

                    /*var user = await _cacheMemory.Get<BaseUserInfo>(_sendingModel.TokenInfo.TokenId, Provider.Globals);
                    if (user == null)
                    {
                        var uInfo = await _authenticateService.Get(_sendingModel.TokenInfo.TokenId);
                        user = uInfo?.UserInfo;
                    }

                    if (user == null)
                    {
                        return StatusCode((int) GlobalStatusCode.AccessDenied,
                            GlobalStatusCode.AccessDenied.GetEnumName());
                    }*/
                }

                var res = await _transmitter.Request(queue, _sendingModel);

                if (res == null)
                {
                    return StatusCode((int) GlobalStatusCode.InternalServerError,
                        GlobalStatusCode.InternalServerError.GetEnumName());
                }

                if (!(res is QWebResponse response))
                {
                    return StatusCode((int) GlobalStatusCode.InternalServerError,
                        GlobalStatusCode.InternalServerError.GetEnumName());
                }

                if (response.Headers == null)
                {
                    response.Headers = new Dictionary<string, string>();
                }

                if (!response.Headers.ContainsKey("Content-Type"))
                {
                    response.Headers.Add("Content-Type", "application/json");
                }

                foreach (var header in response.Headers)
                {
                    Response.Headers.Add(header.Key, header.Value);
                }

                return StatusCode(response.Status, JsonConvert.DeserializeObject(response.Response));
            }
            catch (Exception e)
            {
                
                return StatusCode((int) GlobalStatusCode.InternalServerError, e);
            }
        }
    }
}