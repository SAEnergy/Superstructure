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
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public PropertyGridEditor Editor { get; set; }

        public bool IsIndeterminite { get; set; }

        public bool IsReadOnly { get; set; }

        public bool IsDirty { get; set; }

        public event EventHandler Modified;

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(PropertyGridMetadata), new PropertyMetadata(OnDataChanged));
        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyGridMetadata meta = ((PropertyGridMetadata)d);
            meta.IsDirty = true;
            meta.IsIndeterminite = false;
            if (meta.Modified != null) { meta.Modified(meta, null); }
        }
    }
}
