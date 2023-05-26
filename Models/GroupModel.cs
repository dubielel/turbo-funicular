using System.ComponentModel.DataAnnotations;

namespace turbo_funicular.Models {
    public class Group {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreateTime { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime UpdateTime { get; set; }
    }
}