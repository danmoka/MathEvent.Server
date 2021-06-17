using MathEvent.Converters.Identities.DTOs;
using MathEvent.Converters.Organizations.DTOs;
using System;
using System.Collections.Generic;

namespace MathEvent.Converters.Events.DTOs
{
    /// <summary>
    /// Transfer объект сущности события с пользователями
    /// </summary>
    public class EventWithUsersDTO
    {
        public EventWithUsersDTO()
        {
            ApplicationUsers = new HashSet<UserDTO>();
            Managers = new HashSet<UserDTO>();
        }

        public int Id { get; set; }

        public string AvatarPath { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public OrganizationDTO Organization { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        public int? ParentId { get; set; }
        #endregion

        public ICollection<UserDTO> ApplicationUsers { get; set; }

        public ICollection<UserDTO> Managers { get; set; }
    }
}
