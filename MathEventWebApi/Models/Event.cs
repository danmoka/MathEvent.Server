using System.ComponentModel.DataAnnotations;

namespace MathEventWebApi.Models
{
     public class Event
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(250)]
        public string name { get; set; }

        [Required]
        [MaxLength(450)]
        public string annotation { get; set; }
    }
}