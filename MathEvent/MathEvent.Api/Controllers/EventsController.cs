using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Entities.Models.Events;
using MathEvent.Converters.Events.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;

        public EventsController(IRepositoryWrapper repositoryWrapper, IMapper mapper)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
        }

        // GET api/Events
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventReadDTO>>> ListAsync()
        {
            var eventModels = await _repositoryWrapper.Event.FindAll()
                .Include(ev => ev.ApplicationUsers)
                .ToListAsync();

            if (eventModels != null)
            {
                return Ok(_mapper.Map<IEnumerable<EventReadDTO>>(eventModels));
            }

            return NotFound();
        }

        // GET api/Events/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<EventReadDTO>> RetriveAsync(int id)
        {
            var eventModel = await _repositoryWrapper.Event
                .FindByCondition(ev => ev.Id == id)
                .Include(ev => ev.ApplicationUsers)
                .SingleOrDefaultAsync();

            if (eventModel != null)
            {
                return Ok(_mapper.Map<EventReadDTO>(eventModel));
            }

            return NotFound();
        }

        // POST api/Events
        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] EventCreateDTO eventCreateDTO)
        {
            var eventModel = _mapper.Map<Event>(eventCreateDTO);

            if (!TryValidateModel(eventModel))
            {
                return ValidationProblem(ModelState);
            }

            await _repositoryWrapper.Event.CreateAsync(eventModel);
            await _repositoryWrapper.SaveAsync();

            return Ok();
        }

        // PUT api/Events/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] EventUpdateDTO eventUpdateDTO)
        {
            var eventModel = await _repositoryWrapper.Event
                .FindByCondition(ev => ev.Id == id)
                .Include(ev => ev.ApplicationUsers)
                .SingleOrDefaultAsync();

            if (eventModel == null)
            {
                return NotFound();
            }

            _mapper.Map(eventUpdateDTO, eventModel);

            if (!TryValidateModel(eventModel))
            {
                return ValidationProblem(ModelState);
            }

            _repositoryWrapper.Event.Update(eventModel);
            await _repositoryWrapper.SaveAsync();

            return Ok();
        }

        //PATCH api/Events/{id}
        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialUpdateAsync(int id, [FromBody] JsonPatchDocument<EventUpdateDTO> patchDocument)
        {
            var eventModel = await _repositoryWrapper.Event
                .FindByCondition(ev => ev.Id == id)
                .Include(ev => ev.ApplicationUsers)
                .SingleOrDefaultAsync(); ;

            if (eventModel == null)
            {
                return NotFound();
            }

            var eventToPatch = _mapper.Map<EventUpdateDTO>(eventModel);
            patchDocument.ApplyTo(eventToPatch, ModelState);

            if (!TryValidateModel(eventToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(eventToPatch, eventModel);
            _repositoryWrapper.Event.Update(eventModel);
            await _repositoryWrapper.SaveAsync();

            return Ok();
        }

        // DELETE api/Events/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DestroyAsync(int id)
        {
            var eventModel = await _repositoryWrapper.Event
                .FindByCondition(ev => ev.Id == id)
                .SingleOrDefaultAsync();

            if (eventModel == null)
            {
                return NotFound();
            }

            _repositoryWrapper.Event.Delete(eventModel);
            await _repositoryWrapper.SaveAsync();

            return NoContent();
        }
    }
}
