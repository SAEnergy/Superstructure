using Client.Base;
using System;

namespace Client.Main
{
    public partial class MainWindow : DialogBase
    {
        public MainWindow() : base(null)
        {
            Instance = this;
        }

        public static MainWindow Instance { get; private set; }

        protected override void OnClosed(EventArgs e)
        {
            mainWindowView.Dispose();
            base.OnClosed(e);
        }
    }
}
