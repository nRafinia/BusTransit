using System;

namespace Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class QAllowAnonymousAttribute : Attribute
    {
    }
}