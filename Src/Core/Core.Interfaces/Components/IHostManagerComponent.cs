namespace Core.Interfaces.Components
{
    public interface IHostManagerComponent : IComponentBase
    {
        void StartAll();

        void StopAll();

        void RestartAll();

        void Stop<T>();

        void Start<T>();

        void Restart<T>();
    }
}
