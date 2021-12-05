using System;

namespace MathEvent.DTOs.Users
{
    /// <summary>
    /// Transfer объект сущности пользователя
    /// </summary>
    public class UserDTO
    {
        public Guid Id { get; set; }

        public Guid IdentityUserId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public int? OrganizationId { get; set; }
    }
}
