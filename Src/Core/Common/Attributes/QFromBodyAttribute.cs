using System;

namespace Common.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class QFromBodyAttribute : Attribute
    {
    }
}