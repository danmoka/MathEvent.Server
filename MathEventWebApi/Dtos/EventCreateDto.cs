using System.ComponentModel.DataAnnotations;

namespace MathEventWebApi.Dtos
{
    // для передачи данных для создания модели
     public class EventCreateDto
    {
        [Required]
        [MaxLength(250)]
        public string name { get; set; }
        
        [Required]
        [MaxLength(450)]
        public string annotation { get; set; }
    }
}