
namespace MathEvent.Entities.Models.Event.DTOs
{
    /// <summary>
    /// Класс для передачи данных для чтения Event
    /// </summary>
    public class EventReadDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Annotation { get; set; }
    }
}
