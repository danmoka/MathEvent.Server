using MathEvent.Converters.Identities.Models;

namespace MathEvent.Converters.Organizations.Models
{
    public class OrganizationReadModel
    {
        public int Id { get; set; }

        public string ITN { get; set; }

        public string Name { get; set; }

        public UserSimpleReadModel Manager { get; set; }
    }
}
