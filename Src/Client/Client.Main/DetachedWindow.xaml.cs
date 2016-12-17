using Client.Base;
using System;
using System.Collections.Generic;
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

namespace Client.Main
{
    public partial class DetachedWindow : DialogBase
    {
        public DetachedWindow(PanelBase panel, Window owner) : base(owner)
        {
            DataContext = panel;
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            (this.DataContext as PanelBase).Dispose();
            base.OnClosed(e);
        }
    }
}
