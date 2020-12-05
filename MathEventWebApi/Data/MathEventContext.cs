using MathEventWebApi.Data.Identity;
using MathEventWebApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MathEventWebApi.Data
{
    public class MathEventContext : IdentityDbContext<ApplicationUser>
    {
        public MathEventContext(DbContextOptions<MathEventContext> opt) : base(opt)
        {

        }

        public DbSet<Event> Events { get; set; }
    }
}