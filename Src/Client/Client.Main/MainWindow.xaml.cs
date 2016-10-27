using Client.Base;
using System;

namespace Client.Main
{
    public partial class MainWindow : DialogBase
    {
        public MainWindow() : base(null) { }

        protected override void OnClosed(EventArgs e)
        {
            mainWindowView.Dispose();
            base.OnClosed(e);
        }
    }
}
