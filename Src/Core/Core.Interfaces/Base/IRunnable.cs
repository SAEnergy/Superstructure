namespace Core.Interfaces.Base
{
    public interface IRunnable
    {
        bool IsRunning { get; }

        void Start();

        void Stop();
    }
}
