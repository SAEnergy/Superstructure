using System;

namespace Core.Settings
{
    public class ConfigurationAttribute : Attribute
    {
        public string KeyName { get; set; }

        public object DefaultValue { get; set; }
    }
}
