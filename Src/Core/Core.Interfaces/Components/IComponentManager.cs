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

        ComponentMetadata[] GetComponents();

        void StopComponent(int componentId);

        void StartComponent(int componentId);

        void RestartComponent(int componentId);

        void DisableComponent(int componentId);
    }
}
