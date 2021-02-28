using MathEvent.Converters.Identities.DTOs;
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
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        public int? ParentId { get; set; }

        public EventWithUsersDTO Parent { get; set; }
        #endregion

        public virtual ICollection<UserDTO> ApplicationUsers { get; set; }

        public string Description { get; set; }
    }
}
