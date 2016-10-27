using Client.Base;
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
            ClientSettingsEngine.Instance.Load();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ClientSettingsEngine.Instance.Save();
        }

    }
}
