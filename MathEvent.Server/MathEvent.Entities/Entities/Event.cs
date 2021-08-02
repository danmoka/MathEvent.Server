using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathEvent.Entities.Entities
{
    /// <summary>
    /// Сущность события
    /// </summary>
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 1)]
        public string Name { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        [StringLength(100, MinimumLength = 1)]
        public string Location { get; set; }

        public string AvatarPath { get; set; }

        [ForeignKey("Organization")]
        public int? OrganizationId { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        [ForeignKey("Event")]
        public int? ParentId { get; set; }
        #endregion
    }
}
