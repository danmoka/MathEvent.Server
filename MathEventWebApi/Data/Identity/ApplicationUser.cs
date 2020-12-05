using Microsoft.AspNetCore.Identity;

namespace MathEventWebApi.Data.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set;}
    }
}