namespace Core.Interfaces.Logging
{
    public interface ILogMessageFormatter
    {
        string Format(LogMessage message);

        string GetHeader();
    }
}
