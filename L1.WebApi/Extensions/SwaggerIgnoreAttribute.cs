using System;

namespace L1.WebApi.Extensions
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class SwaggerIgnoreAttribute : Attribute
    {
    }
}