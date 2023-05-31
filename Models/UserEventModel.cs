using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace turbo_funicular.Models 
{
    public class UserEvent 
    {
        [Key]
        public int Id { get; set; }
        [ForeignKeyAttribute("User")]
        public int UserId { get; set; }
        [ForeignKeyAttribute("Event")]
        public int EventId { get; set; }
        public User? User { get; set; }
        public Event? Event { get; set; }
    }
}