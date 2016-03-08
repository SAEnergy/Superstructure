using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Client.Controls
{
    // from http://grokys.blogspot.ro/2010/07/mvvm-and-multiple-selection-part-iii.html

    public interface IMultiSelectCollectionView
    {
        void AddControl(Selector selector);
        void RemoveControl(Selector selector);
    }

    public class MultiSelectCollectionView<T> : ListCollectionView, IMultiSelectCollectionView
    {
        public MultiSelectCollectionView(IList list)
            : base(list)
        {
            SelectedItems = new ObservableCollection<T>();
        }

        void IMultiSelectCollectionView.AddControl(Selector selector)
        {
            this.controls.Add(selector);
            SetSelection(selector);
            selector.SelectionChanged += control_SelectionChanged;
        }

        void IMultiSelectCollectionView.RemoveControl(Selector selector)
        {
            if (this.controls.Remove(selector))
            {
                selector.SelectionChanged -= control_SelectionChanged;
            }
        }

        public ObservableCollection<T> SelectedItems { get; private set; }

        void SetSelection(Selector selector)
        {
            MultiSelector multiSelector = selector as MultiSelector;
            ListBox listBox = selector as ListBox;

            if (multiSelector != null)
            {
                multiSelector.SelectedItems.Clear();

                foreach (T item in SelectedItems)
                {
                    multiSelector.SelectedItems.Add(item);
                }
            }
            else if (listBox != null)
            {
                listBox.SelectedItems.Clear();

                foreach (T item in SelectedItems)
                {
                    listBox.SelectedItems.Add(item);
                }
            }
        }

        void control_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.ignoreSelectionChanged)
            {
                bool changed = false;

                this.ignoreSelectionChanged = true;

                try
                {
                    foreach (T item in e.AddedItems)
                    {
                        if (!SelectedItems.Contains(item))
                        {
                            SelectedItems.Add(item);
                            changed = true;
                        }
                    }

                    foreach (T item in e.RemovedItems)
                    {
                        if (SelectedItems.Remove(item))
                        {
                            changed = true;
                        }
                    }

                    if (changed)
                    {
                        foreach (Selector control in this.controls)
                        {
                            if (control != sender)
                            {
                                SetSelection(control);
                            }
                        }
                    }
                }
                finally
                {
                    this.ignoreSelectionChanged = false;
                }
            }
        }

        bool ignoreSelectionChanged;
        List<Selector> controls = new List<Selector>();
    }

    public static class MultiSelect
    {
        static MultiSelect()
        {
            Selector.ItemsSourceProperty.OverrideMetadata(typeof(Selector), new FrameworkPropertyMetadata(ItemsSourceChanged));
        }

        public static bool GetIsEnabled(Selector target)
        {
            return (bool)target.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(Selector target, bool value)
        {
            target.SetValue(IsEnabledProperty, value);
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(MultiSelect),
                new UIPropertyMetadata(IsEnabledChanged));

        static void IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Selector selector = sender as Selector;
            IMultiSelectCollectionView collectionView = selector.ItemsSource as IMultiSelectCollectionView;

            if (selector != null && collectionView != null)
            {
                if ((bool)e.NewValue)
                {
                    collectionView.AddControl(selector);
                }
                else
                {
                    collectionView.RemoveControl(selector);
                }
            }
        }

        static void ItemsSourceChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Selector selector = sender as Selector;

            if (GetIsEnabled(selector))
            {
                IMultiSelectCollectionView oldCollectionView = e.OldValue as IMultiSelectCollectionView;
                IMultiSelectCollectionView newCollectionView = e.NewValue as IMultiSelectCollectionView;

                if (oldCollectionView != null)
                {
                    oldCollectionView.RemoveControl(selector);
                }

                if (newCollectionView != null)
                {
                    newCollectionView.AddControl(selector);
                }
            }
        }
    }
}
