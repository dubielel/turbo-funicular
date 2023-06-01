using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace turbo_funicular.Models {
    public class User
    {   
        public User()
        {
            OwnedGroups = new List<Group>();
            OwnedEvents = new List<Event>();
            UserGroups = new List<UserGroup>();
            UserEvents = new List<UserEvent>();
            Messages = new List<Message>();
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
    }
}