using System;

namespace Hangfire.Pipelines.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AfterMaterializationAttribute : Attribute
    {
    }
}