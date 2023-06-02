using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using turbo_funicular.Data;

namespace turbo_funicular.Models {
    public class User
    {   
        public User()
        {
            OwnedGroups = new Collection<Group>();
            OwnedEvents = new Collection<Event>();
            UserGroups = new Collection<UserGroup>();
            UserEvents = new Collection<UserEvent>();
            Messages = new Collection<Message>();
        }
        
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreateDate { get; set; }

        public virtual ICollection<Group> OwnedGroups { get; set; }
        public virtual ICollection<Event> OwnedEvents { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }
        public virtual ICollection<UserEvent> UserEvents { get; set; }
        public virtual ICollection<Message> Messages { get; set; }

        public void SetPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                PasswordHash = Convert.ToBase64String(hashedBytes);
            }
        }

        public bool VerifyPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hashedPassword = Convert.ToBase64String(hashedBytes);
                return PasswordHash == hashedPassword;
            }
        }

        public bool isInGroup(ApplicationDbContext dbContext, int groupId)
        {
            var obj = dbContext.UserGroups.FirstOrDefault(e => (e.UserId == Id && e.GroupId == groupId));
            return obj != null;
        }

        public bool isInEvent(ApplicationDbContext dbContext, int eventId)
        {
            var obj = dbContext.UserEvents.FirstOrDefault(e => (e.UserId == Id && e.EventId == eventId));
            return obj != null;
        }

        public List<Group> GetOwnedGroups(ApplicationDbContext dbContext)
        {
            return dbContext.Groups.Where(e => e.UserId == Id).ToList();
        }

        public List<Event> GetOwnedEvents(ApplicationDbContext dbContext)
        {
            return dbContext.Events.Where(e => e.UserId == Id).ToList();
        }

        public List<Message> GetMesseges(ApplicationDbContext dbContext)
        {
            return dbContext.Messages.Where(e => e.UserId == Id).ToList();
        }

        public List<UserGroup> GetUserGroups(ApplicationDbContext dbContext)
        {
            return dbContext.UserGroups.Where(e => e.UserId == Id).ToList();
        }

        public List<UserEvent> GetUserEvents(ApplicationDbContext dbContext)
        {
            return dbContext.UserEvents.Where(e => e.UserId == Id).ToList();
        }
    }
}