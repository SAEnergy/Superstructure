namespace Core.Interfaces.Base
{
    /// <summary>
    /// IoC ComponentRegister will automatically call Initialize when a Component implements this interface.
    /// </summary>
    public interface IInitializable
    {
        bool IsInitialized { get; }

        void Initialize();
    }
}
