using MathEvent.Entities.Models.Events;
using MathEvent.Entities.Models.Identities;
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
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // настройка ссылки таблицы на саму себя
            builder.Entity<Event>()
                .HasOne(e => e.Parent)
                .WithMany()
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // смена названия промежуточной таблицы
            builder.Entity<Event>()
                .HasMany(e => e.ApplicationUsers)
                .WithMany(p => p.Events)
                .UsingEntity(j => j.ToTable("Subscription"));
        }
    }
}
