using System;

namespace Common.Attributes
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class QHttpMethodAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class QGetAttribute : QHttpMethodAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class QPostAttribute : QHttpMethodAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class QPutAttribute : QHttpMethodAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class QDeleteAttribute : QHttpMethodAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class QPatchAttribute : QHttpMethodAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class QHeadAttribute : QHttpMethodAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class QOptionsAttribute : QHttpMethodAttribute
    {
    }
}