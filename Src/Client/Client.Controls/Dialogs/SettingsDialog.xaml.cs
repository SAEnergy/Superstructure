using System;
using Client.Base;
using System.Windows;

namespace Client.Controls
{
    public partial class SettingsDialog : DialogBase
    {
        public SettingsDialog(Window owner) : base(owner)
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            view.Dispose();
            base.OnClosed(e);
        }
    }
}
