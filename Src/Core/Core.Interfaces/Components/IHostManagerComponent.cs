using Core.Interfaces.Base;
using Core.Interfaces.Components.Base;

namespace Core.Interfaces.Components
{
    public interface IHostManagerComponent : IRunnable, IComponentBase
    {
        void RestartAll();

        void Stop<T>();

        void Start<T>();

        void Restart<T>();
    }
}
