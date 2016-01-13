using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Base
{
    public class DialogBase : GlassWindow
    {
        public DialogBase(Window owner)
        {
            this.Owner = owner;
            this.Loaded += DialogBase_Loaded;
        }

        private void DialogBase_Loaded(object sender, RoutedEventArgs e)
        {
            ClientSettingsEngine.GetInstance<WindowPositionSettings>().Unserialize(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ClientSettingsEngine.GetInstance<WindowPositionSettings>().Serialize(this);
        }
    }
}
