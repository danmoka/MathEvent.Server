using System.Collections.Generic;
using MathEventWebApi.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace MathEventWebApi.Data
{
    // класс, который работает с контекстом
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

        public async void CreateAsync(Event ev)
        {
            if(ev == null)
            {
                throw new ArgumentNullException(nameof(ev));
            }

            await _context.Events.AddAsync(ev);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0 );
        }

        public void Update(Event ev)
        {
            //Nothing
        }

        public void Destroy(Event ev)
        {
            if(ev == null)
            {
                throw new ArgumentNullException(nameof(ev));
            }

            _context.Events.Remove(ev);
        }
    }
}