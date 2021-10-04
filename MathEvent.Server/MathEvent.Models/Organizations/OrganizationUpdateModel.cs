namespace MathEvent.Models.Organizations
{
    /// <summary>
    /// Класс для передачи данных для обновления организации
    /// </summary>
    public class OrganizationUpdateModel
    {
        public string ITN { get; set; }

        public string Name { get; set; }

        public string ManagerId { get; set; }
    }
}
