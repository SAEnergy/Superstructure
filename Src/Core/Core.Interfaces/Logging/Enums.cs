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

        protected UserExtensibleEnum(int code, [CallerMemberName] string name = "")
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
        public static LogMessageSeverity Information = new LogMessageSeverity(100);
        public static LogMessageSeverity Warning =     new LogMessageSeverity(400);
        public static LogMessageSeverity Error =       new LogMessageSeverity(1600);
        public static LogMessageSeverity Critical =    new LogMessageSeverity(6400);

        public LogMessageSeverity(int code, [CallerMemberName] string name = "") : base(code,name) { }
    }
}
