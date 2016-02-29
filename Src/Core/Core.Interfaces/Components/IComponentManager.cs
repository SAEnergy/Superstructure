using Core.Interfaces.Components.Base;
using Core.Models.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Components
{
    public interface IComponentManager : IComponentBase
    {
        void StartAll();

        void StopAll();

        ComponentInfo[] GetComponents();

        void StopComponent(ComponentInfo info);

        void StartComponent(ComponentInfo info);
    }
}
