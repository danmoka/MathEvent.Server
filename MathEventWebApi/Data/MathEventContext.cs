using MathEventWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MathEventWebApi.Data
{
    public class MathEventContext : DbContext 
    {
        public MathEventContext(DbContextOptions<MathEventContext> opt) : base(opt)
        {

        }

        public DbSet<Event> Events { get; set; }
    }
}