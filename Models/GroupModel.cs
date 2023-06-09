using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using turbo_funicular.Data;

namespace turbo_funicular.Models {
    public class Group {

        public Group()
        {
            Messages = new Collection<Message>();
            UserGroups = new Collection<UserGroup>();
        }

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

        public List<Message> GetMesseges(ApplicationDbContext dbContext)
        {
            var messages = dbContext.Messages.Where(e => e.GroupId == Id).ToList();

            foreach (var message in messages)
            {
                message.User = dbContext.Users.Find(message.UserId);
                message.Group = dbContext.Groups.Find(message.GroupId);
            }

            return messages;
        }

        public List<UserGroup> GetUserGroups(ApplicationDbContext dbContext)
        {
            var userGroups = dbContext.UserGroups.Where(e => e.GroupId == Id).ToList();

            foreach (var userGroup in userGroups)
            {
                userGroup.User = dbContext.Users.Find(userGroup.UserId);
                userGroup.Group = dbContext.Groups.Find(userGroup.GroupId);
            }

            return userGroups;
        }
    }
}