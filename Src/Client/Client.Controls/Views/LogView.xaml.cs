using System.Windows.Controls;

namespace Client.Controls
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : Client.Base.ViewBase
    {
        public LogView()
        {
            ViewModel = new LogViewModel(this);
            InitializeComponent();
        }

        private void datagrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // If the entire contents fit on the screen, ignore this event
            if (e.ExtentHeight < e.ViewportHeight)
                return;

            // If no items are available to display, ignore this event
            if (logDataGrid.Items.Count <= 0)
                return;

            // If the ExtentHeight and ViewportHeight haven't changed, ignore this event
            if (e.ExtentHeightChange == 0.0 && e.ViewportHeightChange == 0.0)
                return;

            // If we were close to the bottom when a new item appeared,
            // scroll the new item into view.  We pick a threshold of 5
            // items since issues were seen when resizing the window with
            // smaller threshold values.
            var oldExtentHeight = e.ExtentHeight - e.ExtentHeightChange;
            var oldVerticalOffset = e.VerticalOffset - e.VerticalChange;
            var oldViewportHeight = e.ViewportHeight - e.ViewportHeightChange;
            if (oldVerticalOffset + oldViewportHeight + 5 >= oldExtentHeight)
                logDataGrid.ScrollIntoView(logDataGrid.Items[logDataGrid.Items.Count - 1]);
        }
    }
}
