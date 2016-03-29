namespace Core.Interfaces.Base
{
    /// <summary>
    /// Indicates there is a worker thread that can be stopped and started.  Used by Component Manager to start and stop all components.
    /// </summary>
    public interface IRunnable
    {
        bool IsRunning { get; }

        void Start();

        void Stop();
    }
}
