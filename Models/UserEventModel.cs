using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace turbo_funicular.Models {
    public class UserEvent {
        [ForeignKeyAttribute("User")]
        public int UserId { get; set; }
        [ForeignKeyAttribute("Event")]
        public int EventId { get; set; }
        public User? User { get; set; }
        public Event? Event { get; set; }
    }
}