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
