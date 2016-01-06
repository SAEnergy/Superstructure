using System.Runtime.Serialization;

namespace Core.Models.Persistent
{
    [DataContract]
    public class User
    {
        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public string UserName { get; set; }
    }
}
