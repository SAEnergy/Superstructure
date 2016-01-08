using Core.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IHostManagerService
    {
        void StartAll();

        void StopAll();

        void RestartAll();

        void Stop<T>() where T : class, IHost;

        void Start<T>() where T : class, IHost;

        void Restart<T>() where T : class, IHost;
    }
}
