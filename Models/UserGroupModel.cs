using System.ComponentModel.DataAnnotations;

namespace turbo_funicular.Models {
    public class UserGroup {
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public User? User { get; set; }
        public Group? Group { get; set; }
        public bool IsModerator { get; set; }
    }
}