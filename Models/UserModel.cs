using System.ComponentModel.DataAnnotations;

namespace turbo_funicular.Models {
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreateDate { get; set; }

        public virtual ICollection<UserGroup> Groups { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}