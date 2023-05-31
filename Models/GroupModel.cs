using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace turbo_funicular.Models {
    public class Group {
        [Key]
        public int Id { get; set; }
        [ForeignKeyAttribute("User")]
        public int UserId { get; set; }
        public User? User { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreateTime { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime UpdateTime { get; set; }

        public virtual ICollection<UserGroup> UserGroups { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}