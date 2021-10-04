using MathEvent.Models.Users;

namespace MathEvent.Models.Organizations
{
    public class OrganizationReadModel
    {
        public int Id { get; set; }

        public string ITN { get; set; }

        public string Name { get; set; }

        public UserReadModel Manager { get; set; }
    }
}
