using Client.Base;
using Core.Interfaces.Components.Logging;
using Core.IoC.Container;
using Core.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Main
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            IoCContainer.Instance.Register(typeof(Logger));
            IoCContainer.Instance.Resolve<ILogger>().Start();
            ClientSettingsEngine.Instance.Load();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ClientSettingsEngine.Instance.Save();
        }

    }
}
