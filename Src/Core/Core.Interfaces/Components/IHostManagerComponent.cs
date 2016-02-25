using Core.Interfaces.Base;

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
