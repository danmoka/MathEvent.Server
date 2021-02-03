using MathEvent.Entities.Models.Event;
using MathEvent.Entities.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MathEvent.Entities
{
    /// <summary>
    /// Контекст данных для работы с базой данных
    /// </summary>
    public class RepositoryContext : IdentityDbContext<ApplicationUser>
    {
        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options)
        {

        }

        public DbSet<Event> Events { get; set; }
    }
}
