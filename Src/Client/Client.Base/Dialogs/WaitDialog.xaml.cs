﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Base
{
    /// <summary>
    /// Interaction logic for WaitDialog.xaml
    /// </summary>
    public partial class WaitDialog : DialogBase
    {

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(WaitDialog));
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(WaitDialog));
        public static readonly DependencyProperty CurrentValueProperty = DependencyProperty.Register("CurrentValue", typeof(double), typeof(WaitDialog));
        public static readonly DependencyProperty StatusTextProperty = DependencyProperty.Register("StatusText", typeof(string), typeof(WaitDialog));

        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public double CurrentValue
        {
            get { return (double)GetValue(CurrentValueProperty); }
            set { SetValue(CurrentValueProperty, value); }
        }
        public string StatusText
        {
            get { return (string)GetValue(StatusTextProperty); }
            set { SetValue(StatusTextProperty, value); }
        }

        public bool IsCancellable { get; set; }

        public WaitDialog(Window owner) : base(owner)
        {
            this.DataContext = this;

            IsCancellable = true;
            StatusText = "Loading...";
            MinValue = 0;
            MaxValue = 1;
            CurrentValue = 0;
            PersistWindowPosition = false;

            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!IsCancellable)
            {
                e.Cancel = true;
                return;
            }
            base.OnClosing(e);
        }
    }
}
