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

        protected bool PersistWindowPosition { get; set; }

        public DialogBase(Window owner)
        {
            if (Icon == null && owner!=null)
            {
                Icon = owner.Icon;
            }
            PersistWindowPosition = true;
            this.Owner = owner;
            this.Loaded += DialogBase_Loaded;
        }

        public DialogBase()
        {
            throw new InvalidOperationException("This constructor only exists to remove design time warnings, don't use it.");
        }

        private void DialogBase_Loaded(object sender, RoutedEventArgs e)
        {
            if (PersistWindowPosition)
            {
                ClientSettingsEngine.GetInstance<WindowPositionSettings>().Unserialize(this);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (PersistWindowPosition)
            {
                ClientSettingsEngine.GetInstance<WindowPositionSettings>().Serialize(this);
            }
        }

        protected void InvokeIfRequired(Action task)
        {
            if (this.Dispatcher.CheckAccess())
            {
                task();
            }
            else
            {
                Dispatcher.Invoke(task);
            }
        }

        protected void BeginInvokeIfRequired(Action task)
        {
            if (this.Dispatcher.CheckAccess())
            {
                task();
            }
            else
            {
                Dispatcher.BeginInvoke(task);
            }
        }
    }
}
