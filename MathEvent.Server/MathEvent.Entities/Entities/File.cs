using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathEvent.Entities.Entities
{
    /// <summary>
    /// Сущность файла
    /// </summary>
    public class File
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 1)]
        public string Name { get; set; }

        // Папки имеют NULL рарсширение
        [StringLength(5, MinimumLength = 1)]
        public string Extension { get; set; }

        [Required]
        public DateTime Date { get; set; }

        // Папки имеют NULL путь
        public string Path { get; set; }

        [ForeignKey("ApplicationUser")]
        public string AuthorId { get; set; }

        [ForeignKey("FileOwner")]
        public int? OwnerId { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        [ForeignKey("File")]
        public int? ParentId { get; set; }
        #endregion
    }
}
