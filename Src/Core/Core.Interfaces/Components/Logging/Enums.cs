using System.Runtime.CompilerServices;
using Core.Interfaces.Base;

namespace Core.Interfaces.Components.Logging
{
    public class LogMessageSeverity : UserExtensibleEnum
    {
        public static LogMessageSeverity Information = new LogMessageSeverity(10000);
        public static LogMessageSeverity Warning =     new LogMessageSeverity(20000);
        public static LogMessageSeverity Error =       new LogMessageSeverity(30000);
        public static LogMessageSeverity Critical =    new LogMessageSeverity(40000);

        public LogMessageSeverity(int code, [CallerMemberName] string name = "") : base(code,name) { }
    }

    public class LogMessageCategory : UserExtensibleEnum
    {
        public static LogMessageCategory Debug = new LogMessageCategory(10000);
        public static LogMessageCategory Verbose = new LogMessageCategory(20000);
        public static LogMessageCategory Messaging = new LogMessageCategory(30000);
        public static LogMessageCategory General = new LogMessageCategory(40000);
        public static LogMessageCategory Service = new LogMessageCategory(50000);

        public LogMessageCategory(int code, [CallerMemberName] string name = "") : base(code,name) { }
    }
}
