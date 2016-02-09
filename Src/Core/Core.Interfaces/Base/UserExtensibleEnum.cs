namespace Core.Interfaces.Base
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
}
