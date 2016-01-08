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

        void Stop<T>();

        void Start<T>();

        void Restart<T>();
    }
}
