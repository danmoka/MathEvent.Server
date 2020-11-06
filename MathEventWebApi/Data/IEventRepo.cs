using System.Collections.Generic;
using System.Threading.Tasks;
using MathEventWebApi.Models;

namespace MathEventWebApi.Data
{
    public interface IEventRepo
    {
        Task<IEnumerable<Event>> ListAsync();
        Task<Event> RetriveAsync(int id);
    }
}