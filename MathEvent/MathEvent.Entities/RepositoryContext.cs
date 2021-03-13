using MathEvent.Entities.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

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
                .HasOne<Event>()
                .WithMany()
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<ApplicationUser>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();

            // подписки
            builder.Entity<Subscription>()
                .HasKey(s => new { s.ApplicationUserId, s.EventId });
            builder.Entity<Subscription>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(s => s.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Subscription>()
                .HasOne<Event>()
                .WithMany()
                .HasForeignKey(s => s.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // менеджмент
            builder.Entity<Management>()
                .HasKey(m => new { m.ApplicationUserId, m.EventId });
            builder.Entity<Management>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(m => m.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Management>()
                .HasOne<Event>()
                .WithMany()
                .HasForeignKey(m => m.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // файлы
            builder.Entity<File>()
                .HasOne<File>()
                .WithMany()
                .HasForeignKey(f => f.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<File>()
                .HasOne<Owner>()
                .WithMany()
                .HasForeignKey(f => f.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // владельцы
            builder.Entity<Owner>()
                .HasOne<Event>()
                .WithOne()
                .HasForeignKey<Owner>(ow => ow.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Owner>()
                .HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<Owner>(ow => ow.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Entity<Owner>()
                .Property(e => e.OwnedType)
                .HasConversion(
                    v => v.ToString(),
                    v => (Owner.Type)Enum.Parse(typeof(Owner.Type), v));
        }
    }
}
