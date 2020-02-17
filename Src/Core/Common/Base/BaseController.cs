using Microsoft.AspNetCore.Mvc;

namespace Common.Base
{
    public class BaseAnaController:Controller
    {
        protected T Resolve<T>()
        {
            return (T)HttpContext.RequestServices.GetService(typeof(T));
        }
    }
}