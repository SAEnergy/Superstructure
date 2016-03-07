using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Client.Controls
{
    public class PropertyGrid : ItemsControl
    {

        public static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register("Properties", typeof(ObservableCollection<PropertyGridMetadata>), typeof(PropertyGrid));

        public ObservableCollection<PropertyGridMetadata> Properties
        {
            get { return (ObservableCollection<PropertyGridMetadata>)GetValue(PropertiesProperty); }
            set { SetValue(PropertiesProperty, value); }
        }

        public object Instance { get; protected set; }

        static PropertyGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid), new FrameworkPropertyMetadata(typeof(PropertyGrid)));
        }

        public PropertyGrid()
        {
            Properties = new ObservableCollection<PropertyGridMetadata>();
            DataContextChanged += PropertyGrid_DataContextChanged;
        }

        private void PropertyGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Properties.Clear();
            Instance = null;
            if (DataContext == null) { return; }
            Instance = DataContext;
            ParseProperties(Instance.GetType());
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            Properties.Clear();
            Instance = null;

            if (this.Items == null || Items.Count == 0) { return; }

            Type type = Items[0].GetType();
            Instance = Items[0];
            //Instance = Activator.CreateInstance(type);

            foreach (object obj in Items) { if (obj.GetType() != type) { throw new InvalidOperationException("All objects must be of the same type!"); } }

            ParseProperties(type);
        }

        private void ParseProperties(Type type)
        {
            foreach (PropertyInfo prop in Instance.GetType().GetProperties())
            {
                PropertyGridMetadata meta = new PropertyGridMetadata();
                meta.Property = prop;
                meta.IsReadOnly = prop.SetMethod == null || !prop.SetMethod.IsPublic;
                Binding b = new Binding();
                if (!meta.IsReadOnly) { b.Mode = BindingMode.TwoWay; }
                b.Source = Instance;
                b.Path = new PropertyPath(prop.Name);

                BindingOperations.SetBinding(meta, PropertyGridMetadata.DataProperty, b);
                //meta.Data = prop.GetValue(Instance, null);
                meta.Editor = PropertyGridEditorFactory.GetEditor(prop.PropertyType);
                Properties.Add(meta);
            }
        }
    }
}
