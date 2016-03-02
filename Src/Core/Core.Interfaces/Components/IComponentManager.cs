using Core.Interfaces.Components.Base;
using Core.Models.DataContracts;

namespace Core.Interfaces.Components
{
    public interface IComponentManager : IComponentBase
    {
        void StartAll();

        void StopAll();

        ComponentMetadata[] GetComponents();

        ComponentMetadata StopComponent(int componentId);

        ComponentMetadata StartComponent(int componentId);

        ComponentMetadata RestartComponent(int componentId);

        ComponentMetadata DisableComponent(int componentId);
    }
}
