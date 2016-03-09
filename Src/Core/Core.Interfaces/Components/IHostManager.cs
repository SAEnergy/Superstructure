using Core.Interfaces.Base;
using Core.Interfaces.Components.Base;

namespace Core.Interfaces.Components
{
    public interface IHostManager : IRunnable, IComponent
    {
        void RestartAll();

        void Stop<T>();

        void Start<T>();

        void Restart<T>();
    }
}
