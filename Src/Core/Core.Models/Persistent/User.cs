using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Core.Models.Persistent
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        //[Index(IsUnique = true)] //this would require a reference to entity framework in the model, we can also make it unique in the context for the server.
        [StringLength(255)]
        public string UserName { get; set; }
    }
}
