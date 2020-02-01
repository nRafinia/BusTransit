using System;

namespace Common.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public class QFromHeaderAttribute : Attribute
    {
    }
}