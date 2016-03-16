using Core.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.Specialized;
using System.Windows.Threading;
using Client.Base;

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

        private Dictionary<string, PropertyGridMetadata> _properties;

        static PropertyGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid), new FrameworkPropertyMetadata(typeof(PropertyGrid)));
        }

        public PropertyGrid()
        {
            IsTabStop = false;
            Properties = new ObservableCollection<PropertyGridMetadata>();
            _properties = new Dictionary<string, PropertyGridMetadata>();
            DataContextChanged += PropertyGrid_DataContextChanged;
        }

        private void Clear()
        {
            Properties.Clear();
            _properties.Clear();
        }

        private void PropertyGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Clear();

            if (DataContext == null) { return; }

            ParseProperties(new object[] { DataContext });
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            Clear();

            if (this.Items == null || Items.Count == 0) { return; }

            ParseProperties(Items.OfType<object>().ToArray());
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            Clear();

            if (this.Items == null || Items.Count == 0) { return; }

            ParseProperties(Items.OfType<object>().ToArray());
        }

        private void ParseProperties(IEnumerable<object> items)
        {
            Clear();
            foreach (object obj in items)
            {
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (prop.DeclaringType == typeof(DependencyObject)) { continue; }
                    if (prop.DeclaringType == typeof(DispatcherObject)) { continue; }

                    PropertyEditorMetadataAttribute atty = prop.GetCustomAttribute<PropertyEditorMetadataAttribute>();
                    if (atty!=null && atty.Hidden) { continue; }

                    PropertyGridMetadata meta = null;
                    _properties.TryGetValue(prop.Name, out meta);

                    if (meta == null)
                    {
                        meta = new PropertyGridMetadata();
                        meta.Name = prop.Name;
                        meta.DisplayName = PascalCaseSplitter.Split(prop.Name);
                        meta.IsReadOnly = prop.SetMethod == null || !prop.SetMethod.IsPublic;
                        meta.Editor = PropertyGridEditorFactory.GetEditor(prop.PropertyType);
                        meta.Data = prop.GetValue(obj, null);
                        meta.Modified += Meta_Modified;

                        _properties.Add(prop.Name, meta);
                        Properties.Add(meta);

                        continue;
                    }


                }
            }
        }

        private void Meta_Modified(object sender, EventArgs e)
        {
            PropertyGridMetadata meta = sender as PropertyGridMetadata;
            if (meta == null) { return; }

            List<object> items = new List<object>();
            if (Items != null && Items.Count > 0) { items.AddRange(Items.OfType<object>()); }
            else if (DataContext != null) { items.Add(DataContext); }

            if (items.Count == 0) { return; }

            foreach (object obj in items)
            {
                PropertyInfo prop = obj.GetType().GetProperty(meta.Name);
                if (prop == null) { continue; }
                prop.SetValue(obj, Convert.ChangeType(meta.Data,prop.PropertyType));
            }
        }
    }
}
