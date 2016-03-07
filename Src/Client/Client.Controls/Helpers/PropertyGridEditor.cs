using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Client.Controls
{
    public abstract class PropertyGridEditor : Control
    {
        public bool IsIndeterminate;
        static PropertyGridEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditor), new FrameworkPropertyMetadata(typeof(PropertyGridEditor)));
        }
    }

    public abstract class PropertyGridEditor<T> : PropertyGridEditor
    {
    }

    //public abstract class PropertyGridEditor<T> : PropertyGridEditor
    //{
    //    public T CurrentValue;
    //    public bool IsIndeterminate;
    //}

    public class PropertyGridTextEditor : PropertyGridEditor<string>
    {
        static PropertyGridTextEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridTextEditor), new FrameworkPropertyMetadata(typeof(PropertyGridTextEditor)));
        }
    }

}
