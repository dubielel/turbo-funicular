using System.ComponentModel.DataAnnotations;

namespace turbo_funicular.Models {
    public class Group {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreateTime { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime UpdateTime { get; set; }

        public virtual ICollection<User> Members { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}