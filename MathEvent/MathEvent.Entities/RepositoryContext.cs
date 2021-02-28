using MathEvent.Entities.Entities;
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
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // настройка ссылки таблицы на саму себя
            builder.Entity<Event>()
                .HasOne(e => e.Parent)
                .WithMany()
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<ApplicationUser>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();

            // смена названия промежуточной таблицы
            //builder.Entity<Event>()
            //    .HasMany(e => e.ApplicationUsers)
            //    .WithMany(p => p.Events)
            //    .UsingEntity(j => j.ToTable("Subscription"));

            builder.Entity<Subscription>().HasKey(s => new { s.ApplicationUserId, s.EventId });
            builder.Entity<Subscription>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(s => s.ApplicationUserId);
            builder.Entity<Subscription>()
                .HasOne<Event>()
                .WithMany()
                .HasForeignKey(s => s.EventId);
            //builder.Entity<Subscription>()
            //    .HasOne(s => s.ApplicationUserId)
            //    .WithMany(u => u)
            //    .HasForeignKey(s => s.ApplicationUserId);

            //builder.Entity<ApplicationUserPerformance>()
            //    .HasOne(ap => ap.Performance)
            //    .WithMany(p => p.ApplicationUserPerformances)
            //    .HasForeignKey(ap => ap.PerformanceId);
        }
    }
}
