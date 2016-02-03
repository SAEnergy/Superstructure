namespace Core.Interfaces.Components.Logging
{
    public interface ILogMessageFormatter
    {
        string Format(LogMessage message);

        string GetHeader();
    }
}
