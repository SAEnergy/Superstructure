using System;

namespace Core.Settings
{
    public class ConfigurationAttribute : Attribute
    {
        public string KeyName { get; set; }

        public object DefaultValue { get; set; }
    }

    public class ArgumentAttribute : Attribute
    {
        public string Name { get; set; }

        public char Delimiter { get; set; }

        public char Switch { get; set; }

        public bool Optional { get; set; }

        public object DefaultValue { get; set; }

        public ArgumentAttribute()
        {
            Switch = '/';
            Delimiter = '=';
        }
    }
}
