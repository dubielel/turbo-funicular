using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace turbo_funicular.Models {
    public class MessageView {
        public int GroupId { get; set; }
        public string Content { get; set; }
    }
}