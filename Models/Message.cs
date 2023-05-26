using System.ComponentModel.DataAnnotations;

namespace turbo_funicular.Models {
    public class Message {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public User? User { get; set; }
        public Group? Group { get; set; }
        public string Content { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreateDate { get; set; }
    }
}