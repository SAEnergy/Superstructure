using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Client.Base
{
    public class PanelBase : UserControl, IDisposable
    {
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

        public virtual void Dispose()
        {
        }
    }
}
