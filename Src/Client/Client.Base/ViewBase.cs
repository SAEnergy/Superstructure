using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Client.Base
{
    public class ViewBase : UserControl, IDisposable
    {
        private ViewModelBase _viewModel;
        public ViewModelBase ViewModel
        {
            get { return _viewModel; }
            protected set { _viewModel = value;  DataContext = _viewModel; }
        }

        public ViewBase()
        {
        }

        public virtual void Dispose()
        {
            if (ViewModel != null) { ViewModel.Dispose(); }
        }
    }
}
