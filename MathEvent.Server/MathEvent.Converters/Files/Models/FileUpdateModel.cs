using System.ComponentModel.DataAnnotations;

namespace MathEvent.Converters.Files.Models
{
    /// <summary>
    /// Класс для передачи данных для обновления файла
    /// </summary>
    public class FileUpdateModel
    {
        [Required(ErrorMessage = "Название должно быть задано")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "Длина названия должна быть от 1 до 250 символов")]
        public string Name { get; set; }

        // В данный момент не требуется изменять никакие поля кроме названия
        //#region hierarchy
        //public bool? Hierarchy { get; set; }

        //public int? ParentId { get; set; }
        //#endregion

        //public int? OwnerId { get; set; }
    }
}
