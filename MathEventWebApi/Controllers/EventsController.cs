using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathEventWebApi.Data;
using MathEventWebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MathEventWebApi.Contollers
{
    [Route("/events")]
    [ApiController]
    public class EventsController :  ControllerBase
    {
        private readonly IEventRepo _repository;
        public EventsController(IEventRepo repository)
        {
            _repository = repository;
        }

        //GET api/events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> ListAsync()
        {
            IEnumerable<Event> events = await _repository.ListAsync();

            if (events != null && events.Any())
            {
                return Ok(events);
            }

            return BadRequest();
        }

        //GET api/events/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> RetriveAsync(int id) 
        {
            Event ev = await _repository.RetriveAsync(id);

            if (ev != null) 
            {
                return Ok(ev);
            }

            return BadRequest();
        }
    }
}