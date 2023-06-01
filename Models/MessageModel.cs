using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace turbo_funicular.Models {
    public class Message {
        [Key]
        public int Id { get; set; }
        [ForeignKeyAttribute("User")]
        public int UserId { get; set; }
        [ForeignKeyAttribute("Group")]
        public int GroupId { get; set; }
        public User? User { get; set; }
        public Group? Group { get; set; }
        public string Content { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreateDate { get; set; }
    }
}