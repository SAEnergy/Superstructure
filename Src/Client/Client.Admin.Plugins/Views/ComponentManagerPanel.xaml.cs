using Client.Base;
using Core.Comm;
using Core.Interfaces.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Client.Admin.Plugins
{
    /// <summary>
    /// Interaction logic for ComponentManagerPanel.xaml
    /// </summary>
    [PanelMetadata(DisplayName = "Component Manager", IconPath = "images/puzzle-piece.png")]
    public partial class ComponentManagerPanel : PanelBase
    {
        public ComponentManagerPanel()
        {
            ViewModel = new ComponentManagerViewModel(this);
            InitializeComponent();
        }
    }
}
