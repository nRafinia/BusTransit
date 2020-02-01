using System;

namespace Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class QAuthAttribute : Attribute
    {
    }
}