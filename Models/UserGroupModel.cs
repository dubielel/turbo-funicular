using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace turbo_funicular.Models {
    public class UserGroup {
        [ForeignKeyAttribute("User")]
        public int UserId { get; set; }
        [ForeignKeyAttribute("Attribute")]
        public int GroupId { get; set; }
        public User? User { get; set; }
        public Group? Group { get; set; }
        public bool IsModerator { get; set; }
    }
}