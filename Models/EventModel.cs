using System.ComponentModel.DataAnnotations;

namespace turbo_funicular.Models {
    public class Event {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public User? User { get; set; }
        public Group? Group { get; set; }
        [DataType(DataType.DateTime)]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public int MaxParticipants { get; set; }

        public virtual ICollection<User> Participants { get; set; }
    }
}