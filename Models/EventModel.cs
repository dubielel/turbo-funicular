using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using turbo_funicular.Data;

namespace turbo_funicular.Models {
    public class Event {
        public Event()
        {
            UserEvents = new Collection<UserEvent>();
        }
        
        [Key]
        public int Id { get; set; }
        [ForeignKeyAttribute("User")]
        public int UserId { get; set; }
        public User? User { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreateDate { get; set; }
        public int MaxParticipants { get; set; }

        public virtual ICollection<UserEvent> UserEvents { get; set; }

        public List<UserEvent> GetUserEvents(ApplicationDbContext dbContext)
        {
            return dbContext.UserEvents.Where(e => e.EventId == Id).ToList();
        }
    }
}