using System;

using JetBrains.Annotations;

namespace Hangfire.Pipelines.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MaterializeValueAttribute : Attribute
    {
        [NotNull]
        public string Key { get; }

        public MaterializeValueAttribute([NotNull] string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));
            }

            Key = key;
        }
    }
}