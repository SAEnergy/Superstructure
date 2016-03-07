using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Controls
{
    public class PropertyGridMetadata : DependencyObject
    {
        public PropertyInfo Property { get; set; }

        public PropertyGridEditor Editor { get; set; }

        public bool IsIndeterminite { get; set; }

        public bool IsReadOnly { get; set; }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(PropertyGridMetadata));
        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
    }
}
