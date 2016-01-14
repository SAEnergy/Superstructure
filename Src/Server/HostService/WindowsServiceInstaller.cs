using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration.Install;
using System.ServiceProcess;
using System.ComponentModel;


namespace HostService
{
    [RunInstaller(true)]
    public class WindowsServiceInstaller : Installer
    {
        private ServiceInstaller _serviceInstaller;
        private ServiceProcessInstaller _serviceProcessInstaller;
        private HostServiceBase _hostServiceBase;

        public WindowsServiceInstaller()
        {
            _serviceProcessInstaller = new ServiceProcessInstaller();
            _serviceInstaller = new ServiceInstaller();
            _hostServiceBase = new HostServiceBase();

            _serviceInstaller.ServiceName = "Host Service";
            _serviceInstaller.StartType = ServiceStartMode.Manual;

            _serviceProcessInstaller.Account = ServiceAccount.LocalSystem;

            _hostServiceBase.ExitCode = 0;
            _hostServiceBase.ServiceName = "Host Service";

            Installers.Add(_serviceProcessInstaller);
            Installers.Add(_serviceInstaller);
        }
    }
}

