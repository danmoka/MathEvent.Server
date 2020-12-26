using System.ComponentModel.DataAnnotations;

namespace MathEvent.Entities.Models.Event
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        [Required]
        [MaxLength(450)]
        public string Annotation { get; set; }
    }
}
