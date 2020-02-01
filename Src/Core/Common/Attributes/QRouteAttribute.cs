using System;

namespace Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class QRouteAttribute : Attribute
    {
        public string Route { get; }

        public QRouteAttribute(string route)
        {
            Route = route;
        }
    }
}