using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Core.Interfaces.Logging
{
    public class UserExtensibleEnum
    {
        public int Code { get; private set; }
        public string Name { get; private set; }

        protected UserExtensibleEnum(int code, string name)
        {
            Code = code;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static bool operator >(UserExtensibleEnum e1, UserExtensibleEnum e2)
        {
            return e1.Code > e2.Code;
        }
        public static bool operator <(UserExtensibleEnum e1, UserExtensibleEnum e2)
        {
            return e1.Code < e2.Code;
        }


    }

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
