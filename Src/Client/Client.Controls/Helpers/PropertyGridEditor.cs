using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Client.Controls
{
    public abstract class PropertyGridEditor : Control
    {
        public PropertyGridEditor()
        {
            IsTabStop = false;
        }
    }

    public class PropertyGridTextEditor : PropertyGridEditor
    {
        static PropertyGridTextEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridTextEditor), new FrameworkPropertyMetadata(typeof(PropertyGridTextEditor)));
        }
    }

    public class PropertyGridBoolEditor : PropertyGridEditor
    {
        static PropertyGridBoolEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridBoolEditor), new FrameworkPropertyMetadata(typeof(PropertyGridBoolEditor)));
        }
    }

    public class PropertyGridEnumEditor : PropertyGridEditor
    {
        static PropertyGridEnumEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEnumEditor), new FrameworkPropertyMetadata(typeof(PropertyGridEnumEditor)));
        }

        public ObservableCollection<object> AvailableValues { get; set; }

        public PropertyGridEnumEditor(Type enumType)
        {
            AvailableValues = new ObservableCollection<object>();
            foreach (object obj in Enum.GetValues(enumType))
            {
                AvailableValues.Add(obj);
            }
        }
    }
}
