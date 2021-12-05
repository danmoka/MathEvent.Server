using System;

namespace MathEvent.DTOs.Files
{
    /// <summary>
    /// Transfer объект сущности файла
    /// </summary>
    public class FileDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public DateTime Date { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        public int? ParentId { get; set; }
        #endregion

        public Guid AuthorId { get; set; }

        public int? OwnerId { get; set; }

        public string Path { get; set; }
    }
}
