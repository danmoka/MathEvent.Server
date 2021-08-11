using MathEvent.Converters.Identities.DTOs;

namespace MathEvent.Converters.Organizations.DTOs
{
    public class OrganizationDTO
    {
        public int Id { get; set; }

        public string ITN { get; set; }

        public string Name { get; set; }

        public UserDTO Manager { get; set; }
    }
}
