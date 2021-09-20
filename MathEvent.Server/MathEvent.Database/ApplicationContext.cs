using MathEvent.Entities.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace MathEvent.Database
{
    /// <summary>
    /// Контекст данных для работы с базой данных
    /// </summary>
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }

        public DbSet<Event> Events { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // события
            // настройка ссылки таблицы на саму себя
            builder.Entity<Event>()
                .HasOne<Event>()
                .WithMany()
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Event>()
                .HasOne<Organization>()
                .WithMany()
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // пользователи
            builder.Entity<ApplicationUser>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
            builder.Entity<ApplicationUser>()
                .HasOne<Organization>()
                .WithMany()
                .HasForeignKey(u => u.OrganizationId)
                .OnDelete(DeleteBehavior.SetNull);

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
                .OnDelete(DeleteBehavior.Restrict);
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

            // перевод типа владельца в строковое значение
            builder.Entity<Owner>()
                .Property(e => e.OwnedType)
                .HasConversion(
                    v => v.ToString(),
                    v => (Owner.Type)Enum.Parse(typeof(Owner.Type), v));

            // организации
            builder.Entity<Organization>()
                .HasIndex(org => org.ITN)
                .IsUnique();
            builder.Entity<Organization>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(org => org.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
