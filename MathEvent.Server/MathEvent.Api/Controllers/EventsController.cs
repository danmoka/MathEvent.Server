using AutoMapper;
using MathEvent.Converters.Events.DTOs;
using MathEvent.Converters.Events.Models;
using MathEvent.Converters.Others;
using MathEvent.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly IEventService _eventService;

        public EventsController(IMapper mapper, IEventService eventService)
        {
            _mapper = mapper;
            _eventService = eventService;
        }

        // GET api/Events/?key1=value1&key2=value2
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventReadModel>>> ListAsync([FromQuery] IDictionary<string, string> filters)
        {
            var eventReadModels = await _eventService.ListAsync(filters);

            if (eventReadModels is not null)
            {
                return Ok(eventReadModels);
            }

            return NotFound();
        }

        // GET api/Events/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<EventWithUsersReadModel>> RetrieveAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }

            var eventReadModel = await _eventService.RetrieveAsync(id);

            if (eventReadModel is not null)
            {
                return Ok(eventReadModel);
            }

            return NotFound();
        }

        // POST api/Events
        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] EventCreateModel eventCreateModel)
        {
            if (!TryValidateModel(eventCreateModel))
            {
                return ValidationProblem(ModelState);
            }

            var createResult = await _eventService.CreateAsync(eventCreateModel);

            if (createResult.Succeeded)
            {
                var createdEvent = createResult.Entity;

                if (createdEvent is null)
                {
                    return Ok();
                }

                return StatusCode(201, createdEvent.Id);
            }
            else
            {
                return BadRequest(createResult.Messages);
            }
        }

        // PUT api/Events/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] EventUpdateModel eventUpdateModel)
        {
            if (id < 0)
            {
                return BadRequest();
            }

            if (await _eventService.GetEventEntityAsync(id) is null)
            {
                return NotFound();
            }

            if (!TryValidateModel(eventUpdateModel))
            {
                return ValidationProblem(ModelState);
            }

            var updateResult = await _eventService.UpdateAsync(id, eventUpdateModel);

            if (updateResult.Succeeded)
            {
                return Ok(id);
            }
            else
            {
                return BadRequest(updateResult.Messages);
            }
        }

        //PATCH api/Events/{id}
        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialUpdateAsync(int id, [FromBody] JsonPatchDocument<EventUpdateModel> patchDocument)
        {
            if (id < 0)
            {
                return BadRequest();
            }

            if (patchDocument is null)
            {
                return BadRequest();
            }

            var eventEntity = await _eventService.GetEventEntityAsync(id);

            if (eventEntity is null)
            {
                return NotFound();
            }

            var eventDTO = _mapper.Map<EventWithUsersDTO>(eventEntity);
            var eventToPatch = _mapper.Map<EventUpdateModel>(eventDTO);
            patchDocument.ApplyTo(eventToPatch, ModelState);

            if (!TryValidateModel(eventToPatch))
            {
                return ValidationProblem(ModelState);
            }

            var updateResult = await _eventService.UpdateAsync(id, eventToPatch);

            if (updateResult.Succeeded)
            {
                return Ok(id);
            }
            else
            {
                return BadRequest(updateResult.Messages);
            }
        }

        // DELETE api/Events/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DestroyAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }

            if (await _eventService.GetEventEntityAsync(id) is null)
            {
                return NotFound();
            }

            var deleteResult = await _eventService.DeleteAsync(id);

            if (deleteResult.Succeeded)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(deleteResult.Messages);
            }
        }

        // GET api/Events/Breadcrumbs/{id}
        [HttpGet("Breadcrumbs/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Breadcrumb>>> GetBreadcrumbs(int id)
        {
            var result = await _eventService.GetBreadcrumbs(id);

            if (result.Succeeded)
            {
                return Ok(result.Entity);
            }
            else
            {
                return BadRequest(result.Messages);
            }
        }
    }
}
