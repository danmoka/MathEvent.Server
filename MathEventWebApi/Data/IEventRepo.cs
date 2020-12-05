using System.Collections.Generic;
using System.Threading.Tasks;
using MathEventWebApi.Models;

namespace MathEventWebApi.Data
{
    public interface IEventRepo
    {
        Task<bool> SaveChangesAsync();
        Task<IEnumerable<Event>> ListAsync();
        Task<Event> RetriveAsync(int id);
        void CreateAsync(Event ev);
        void Update(Event ev);
        void Destroy(Event ev);
    }
}