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

        public string PropertyName { get; set; }

        public Type PropertyType { get; set; }

        public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register("DisplayName", typeof(string), typeof(PropertyGridEditor));
        public virtual string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(PropertyGridEditor));
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty IsMultipleValuesProperty = DependencyProperty.Register("IsMultipleValues", typeof(bool), typeof(PropertyGridEditor));
        public bool IsMultipleValues
        {
            get { return (bool)GetValue(IsMultipleValuesProperty); }
            set { SetValue(IsMultipleValuesProperty, value); }
        }

        public bool IsDirty { get; set; }

        public event EventHandler Modified;

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(PropertyGridEditor), new PropertyMetadata(null, OnDataChanged, OnCoerceData));
        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        private static object OnCoerceData(DependencyObject d, object baseValue)
        {
            PropertyGridEditor ed = ((PropertyGridEditor)d);
            return Convert.ChangeType(baseValue, ed.PropertyType);
        }

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyGridEditor ed = ((PropertyGridEditor)d);
            ed.IsDirty = true;
            if (ed.Modified != null) { ed.Modified(ed, null); }
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


        public static readonly DependencyProperty AvailableValuesProperty = DependencyProperty.Register("AvailableValues", typeof(ObservableCollection<object>), typeof(PropertyGridEditor));
        public ObservableCollection<object> AvailableValues
        {
            get { return (ObservableCollection<object>)GetValue(AvailableValuesProperty); }
            set { SetValue(AvailableValuesProperty, value); }
        }

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
