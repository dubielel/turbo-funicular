using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace turbo_funicular.Models {
    public class Event {
        [Key]
        public int Id { get; set; }
        [ForeignKeyAttribute("User")]
        public int UserId { get; set; }
        public User? User { get; set; }
        [DataType(DataType.DateTime)]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public int MaxParticipants { get; set; }

        public virtual ICollection<User> Participants { get; set; }
    }
}