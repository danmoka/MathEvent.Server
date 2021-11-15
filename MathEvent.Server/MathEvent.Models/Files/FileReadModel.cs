using System;

namespace MathEvent.Models.Files
{
    /// <summary>
    /// Класс для передачи информации о файле
    /// </summary>
    public class FileReadModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public DateTime Date { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        public int? ParentId { get; set; }
        #endregion

        public string AuthorId { get; set; }

        public int? OwnerId { get; set; }
    }
}
