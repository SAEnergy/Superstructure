using System.Runtime.Serialization;

namespace Core.Interfaces.Base
{
    [DataContract]
    public class UserExtensibleEnum
    {
        [DataMember]
        public int Code { get; private set; }
        [DataMember]
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

        public static bool operator ==(UserExtensibleEnum e1, UserExtensibleEnum e2)
        {
            return (e1.GetType() == e2.GetType() && e1.Code == e2.Code);
        }
        public static bool operator !=(UserExtensibleEnum e1, UserExtensibleEnum e2)
        {
            return (e1.GetType() != e2.GetType() || e1.Code != e2.Code);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
