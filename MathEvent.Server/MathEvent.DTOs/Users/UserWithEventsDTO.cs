﻿using MathEvent.DTOs.Events;
using MathEvent.DTOs.Organizations;
using System;
using System.Collections.Generic;

namespace MathEvent.DTOs.Users
{
    /// <summary>
    /// Transfer объект сущности пользователя с событиями
    /// </summary>
    public class UserWithEventsDTO
    {
        public UserWithEventsDTO()
        {
            Events = new HashSet<EventDTO>();
            ManagedEvents = new HashSet<EventDTO>();
        }

        public Guid Id { get; set; }

        public Guid IdentityUserId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public OrganizationDTO Organization { get; set; }

        public virtual ICollection<EventDTO> Events { get; set; }

        public virtual ICollection<EventDTO> ManagedEvents { get; set; }
    }
}
