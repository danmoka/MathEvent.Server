namespace MathEvent.DTOs.Users
{
    /// <summary>
    /// Transfer объект сущности пользователя
    /// </summary>
    public class UserDTO
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Patronymic { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public int? OrganizationId { get; set; }
    }
}
