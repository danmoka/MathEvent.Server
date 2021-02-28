using Microsoft.AspNetCore.Identity;

namespace MathEvent.Entities.Entities
{
    /// <summary>
    /// Сущность пользователя
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Patronymic { get; set; }
    }
}