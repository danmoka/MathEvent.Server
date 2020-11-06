using System.Collections.Generic;
using MathEventWebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MathEventWebApi.Contollers
{
    [Route("/events")]
    [ApiController]
    public class EventsController :  ControllerBase
    {
        public EventsController()
        {

        }

        // //GET api/events
        // [HttpGet]
        // public ActionResult<IEnumerable<Event>> List()
        // {
            
        // }

        // //GET api/events/{id}
        // [HttpGet("{id}")]
        // public ActionResult<Event> Retrive(int id) 
        // {

        // }
    }
}