using System.Collections.Generic;
using System.Linq;
using MathEventWebApi.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MathEventWebApi.Data
{
    public class SqlEventRepo : IEventRepo
    {
        private readonly MathEventContext _context;

        public SqlEventRepo(MathEventContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Event>> ListAsync()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<Event> RetriveAsync(int id)
        {
            return await _context.Events.SingleOrDefaultAsync(e => e.Id == id);
        }
    }
}