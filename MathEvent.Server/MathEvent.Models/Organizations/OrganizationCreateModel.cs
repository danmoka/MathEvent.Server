namespace MathEvent.Models.Organizations
{
    /// <summary>
    /// Класс для передачи данных для создания организации
    /// </summary>
    public class OrganizationCreateModel
    {
        public string ITN { get; set; }

        public string Name { get; set; }
    }
}
