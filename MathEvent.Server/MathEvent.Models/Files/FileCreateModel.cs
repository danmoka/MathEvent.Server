using System;

namespace MathEvent.Models.Files
{
    /// <summary>
    /// Класс для передачи данных для создания файла
    /// </summary>
    public class FileCreateModel
    {
        public string Name { get; set; }

        public Guid AuthorId { get; set; }

        public int? OwnerId { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        public int? ParentId { get; set; }
        #endregion
    }
}
