using System.ComponentModel.DataAnnotations;

namespace MathEvent.Converters.Files.Models
{
    /// <summary>
    /// Класс для передачи данных для создания файла
    /// </summary>
    public class FileCreateModel
    {
        [Required(ErrorMessage = "Имя должно быть задано")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "Длина названия должна быть от 1 до 250 символов")]
        public string Name { get; set; }

        public string AuthorId { get; set; }

        [Required(ErrorMessage = "Неопределен владелец файла")]
        public int? OwnerId { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        public int? ParentId { get; set; }
        #endregion
    }
}
