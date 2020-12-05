using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MathEventWebApi.Data;
using MathEventWebApi.Dtos;
using MathEventWebApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MathEventWebApi.Contollers
{
    [Route("/events")] 
    [ApiController]
    public class EventsController :  ControllerBase
    {
        private readonly IEventRepo _repository;
        private readonly IMapper _mapper;

        public EventsController(IEventRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        //GET api/events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventReadDto>>> ListAsync()
        {
            var events = await _repository.ListAsync();

            if (events != null && events.Any())
            {
                return Ok(_mapper.Map<IEnumerable<EventReadDto>>(events));
            }

            return NotFound();
        }

        //GET api/events/{id}
        [HttpGet("{id}", Name="RetriveAsync")]
        public async Task<ActionResult<EventReadDto>> RetriveAsync(int id) 
        {
            var ev = await _repository.RetriveAsync(id);

            if (ev != null) 
            {
                return Ok(_mapper.Map<EventReadDto>(ev));
            }

            return NotFound();
        }

        //POST api/events
        [HttpPost]
        public async Task<ActionResult<EventReadDto>> CreateAsync(EventCreateDto eventCreateDto)
        {
            var eventModel = _mapper.Map<Event>(eventCreateDto);
            _repository.CreateAsync(eventModel);
            await _repository.SaveChangesAsync();

            var eventReadDto = _mapper.Map<EventReadDto>(eventModel);

            return CreatedAtRoute(nameof(RetriveAsync), new {Id = eventReadDto.Id}, eventReadDto);
        }

        //PUT api/events/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, EventUpdateDto eventUpdateDto)
        {
            var eventModelFromRepo = await _repository.RetriveAsync(id);

            if(eventModelFromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(eventUpdateDto, eventModelFromRepo);
            _repository.Update(eventModelFromRepo); // можно не вызывать, но по лучшей практикой будет вызвать
            await _repository.SaveChangesAsync();
            
            return NoContent();
        }

        //PATCH api/events/{id}
        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialUpdateAsync(int id, JsonPatchDocument<EventUpdateDto> patchDocument)
        {
            var eventModelFromRepo = await _repository.RetriveAsync(id);

            if(eventModelFromRepo == null)
            {
                return NotFound();
            }

            var eventToPatch = _mapper.Map<EventUpdateDto>(eventModelFromRepo);
            patchDocument.ApplyTo(eventToPatch, ModelState);

            if(!TryValidateModel(eventToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(eventToPatch, eventModelFromRepo);
            _repository.Update(eventModelFromRepo);
            await _repository.SaveChangesAsync();
            
            return NoContent();
        }

        //DELETE api/events/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Destroy(int id)
        {
            var eventModelFromRepo = await _repository.RetriveAsync(id);

            if(eventModelFromRepo == null)
            {
                return NotFound();
            }

            _repository.Destroy(eventModelFromRepo);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}