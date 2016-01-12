using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Core.Models.Persistent
{
    [DataContract]
    public class User
    {
        [Key]
        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        //[Index(IsUnique = true)] //this would require a reference to entity framework in the model, we can also make it unique in the context for the server.
        [StringLength(255)]
        public string UserName { get; set; }
    }
}
