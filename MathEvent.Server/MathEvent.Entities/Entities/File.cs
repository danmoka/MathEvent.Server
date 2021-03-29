using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathEvent.Entities.Entities
{
    /// <summary>
    /// Сущность файла
    /// </summary>
    [Table("Files")]
    public class File
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(250)]
        public string Name { get; set; }

        [MaxLength(5)]
        public string Extension { get; set; }

        public DateTime Date { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        [ForeignKey("File")]
        public int? ParentId { get; set; }
        #endregion

        [ForeignKey("ApplicationUser")]
        public string AuthorId { get; set; }

        [ForeignKey("FileOwner")]
        public int? OwnerId { get; set; }

        public string Path { get; set; }
    }
}
