using AutoMapper;
using MathEvent.AuthorizationHandlers;
using MathEvent.Converters.Events.DTOs;
using MathEvent.Converters.Events.Models;
using MathEvent.Converters.Files.Models;
using MathEvent.Converters.Others;
using MathEvent.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MathEvent.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly EventService _eventService;

        private readonly FileService _fileService;

        private readonly UserService _userService;

        private readonly IAuthorizationService _authorizationService;

        public EventsController(
            IMapper mapper,
            EventService eventService,
            FileService fileService,
            UserService userService,
            IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _eventService = eventService;
            _fileService = fileService;
            _userService = userService;
            _authorizationService = authorizationService;
        }

        // GET api/Events/?key1=value1&key2=value2
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventReadModel>>> ListAsync([FromQuery] IDictionary<string, string> filters)
        {
            var eventsResult = await _eventService.ListAsync(filters);

            if (eventsResult.Succeeded)
            {
                var eventReadModels = eventsResult.Entity;

                if (eventReadModels is not null)
                {
                    return Ok(eventReadModels);
                }
            }

            return NotFound(eventsResult.Messages);
        }

        // GET api/Events/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<EventWithUsersReadModel>> RetrieveAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest($"id = {id} less then 0");
            }

            var eventResult = await _eventService.RetrieveAsync(id);

            if (eventResult.Succeeded && eventResult.Entity is not null)
            {
                return Ok(eventResult.Entity);
            }

            return NotFound(eventResult.Messages);
        }

        // POST api/Events
        [HttpPost]
        public async Task<ActionResult<EventWithUsersReadModel>> CreateAsync([FromBody] EventCreateModel eventCreateModel)
        {
            var userResult = await _userService.GetCurrentUserAsync(User);

            if (!userResult.Succeeded || userResult.Entity is null)
            {
                return StatusCode(401);
            }

            eventCreateModel.AuthorId = userResult.Entity.Id;
            var createResult = await _eventService.CreateAsync(eventCreateModel);

            if (createResult.Succeeded)
            {
                var createdEvent = createResult.Entity;

                if (createdEvent is null)
                {
                    return StatusCode(201);
                }

                return StatusCode(201, createdEvent);
            }
            else
            {
                return StatusCode(500, createResult.Messages);
            }
        }

        // PUT api/Events/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] EventUpdateModel eventUpdateModel)
        {
            if (id < 0)
            {
                return BadRequest($"id = {id} less then 0");
            }

            if (eventUpdateModel.Managers.Count < 1)
            {
                return BadRequest("The list of event managers is empty");
            }

            var eventResult = await _eventService.GetEventEntityAsync(id);

            if (!eventResult.Succeeded || eventResult.Entity is null)
            {
                return NotFound(eventResult.Messages);
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, eventResult.Entity, Operations.Update);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(403);
            }

            var updateResult = await _eventService.UpdateAsync(id, eventUpdateModel);

            if (updateResult.Succeeded)
            {
                var updatedEvent = updateResult.Entity;

                if (updatedEvent is null)
                {
                    return Ok(id);
                }

                return Ok(updatedEvent);
            }
            else
            {
                return StatusCode(500, updateResult.Messages);
            }
        }

        //PATCH api/Events/{id}
        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialUpdateAsync(int id, [FromBody] JsonPatchDocument<EventUpdateModel> patchDocument)
        {
            if (id < 0)
            {
                return BadRequest($"id = {id} less then 0");
            }

            if (patchDocument is null)
            {
                return BadRequest("Patch document is null");
            }

            var eventResult = await _eventService.GetEventEntityAsync(id);

            if (!eventResult.Succeeded || eventResult.Entity is null)
            {
                return NotFound(eventResult.Messages);
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, eventResult.Entity, Operations.Update);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(403);
            }

            var eventDTO = _mapper.Map<EventWithUsersDTO>(eventResult.Entity);
            var eventToPatch = _mapper.Map<EventUpdateModel>(eventDTO);
            patchDocument.ApplyTo(eventToPatch, ModelState);

            if (!TryValidateModel(eventToPatch))
            {
                return ValidationProblem(ModelState);
            }

            if (eventToPatch.Managers.Count < 1)
            {
                // такие проверки мб уйдут после изменения валидации для таких полей
                return BadRequest("The list of event managers is empty");
            }

            var updateResult = await _eventService.UpdateAsync(id, eventToPatch);

            if (updateResult.Succeeded)
            {
                var updatedEvent = updateResult.Entity;

                if (updatedEvent is null)
                {
                    return Ok(id);
                }

                return Ok(updatedEvent);
            }
            else
            {
                return StatusCode(500, updateResult.Messages);
            }
        }

        // DELETE api/Events/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DestroyAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest($"id = {id} less then 0");
            }

            var eventResult = await _eventService.GetEventEntityAsync(id);

            if (!eventResult.Succeeded || eventResult.Entity is null)
            {
                return NotFound(eventResult.Messages);
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, eventResult.Entity, Operations.Delete);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(403);
            }

            var childEventsResult = await _eventService.GetChildEvents(id);

            if (childEventsResult.Succeeded)
            {
                return BadRequest(childEventsResult.Messages);
            }

            var deleteResult = await _eventService.DeleteAsync(id);

            if (deleteResult.Succeeded)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(500, deleteResult.Messages);
            }
        }

        // GET api/Events/Breadcrumbs/{id}
        [HttpGet("Breadcrumbs/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Breadcrumb>>> GetBreadcrumbs(int id)
        {
            if (id < 0)
            {
                return BadRequest($"id = {id} less then 0");
            }

            var result = await _eventService.GetBreadcrumbs(id);

            if (result.Succeeded)
            {
                return Ok(result.Entity);
            }
            else
            {
                return StatusCode(500, result.Messages);
            }
        }

        // GET api/Events/Statistics/?key1=value1&key2=value2
        [HttpGet("Statistics/")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SimpleStatistics>>> StatisticsAsync([FromQuery] IDictionary<string, string> filters)
        {
            var eventsStatisticsResult = await _eventService.GetSimpleStatistics(filters);

            if (eventsStatisticsResult.Succeeded && eventsStatisticsResult.Entity is not null)
            {
                return Ok(eventsStatisticsResult.Entity);
            }

            return StatusCode(500, eventsStatisticsResult.Messages);
        }

        // GET api/Events/Statistics/{id}
        [HttpGet("Statistics/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SimpleStatistics>>> EventStatisticsAsync(int id)
        {
            var eventStatisticsResult = await _eventService.GetEventStatistics(id);

            if (eventStatisticsResult.Succeeded && eventStatisticsResult.Entity is not null)
            {
                return Ok(eventStatisticsResult.Entity);
            }

            return StatusCode(500, eventStatisticsResult.Messages);
        }

        // POST api/Events/Avatar/?id=value1
        [HttpPost("Avatar")]
        public async Task<ActionResult> UploadAvatar([FromForm] IFormFile file, [FromQuery] string id)
        {
            int eventId = -1;

            if (int.TryParse(id, out int eventIdParam))
            {
                eventId = eventIdParam;
            }

            if (eventId < 0)
            {
                return BadRequest($"id = {id} less then 0");
            }

            var eventResult = await _eventService.GetEventEntityAsync(eventId);

            if (!eventResult.Succeeded || eventResult.Entity is null)
            {
                return NotFound(eventResult.Messages);
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, eventResult.Entity, Operations.Update);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(403);
            }

            var checkResult = _fileService.IsCorrectImage(file);

            if (!checkResult.Succeeded)
            {
                return BadRequest(checkResult.Messages);
            }

            var userResult = await _userService.GetCurrentUserAsync(User);

            if (!userResult.Succeeded)
            {
                return NotFound(userResult.Messages);
            }

            var user = userResult.Entity;

            if (user is null)
            {
                return StatusCode(500, "Entity is null");
            }

            var fileCreateModel = new FileCreateModel
            {
                Name = Path.GetFileNameWithoutExtension(file.FileName),
                Hierarchy = null,
                ParentId = null,
                AuthorId = user.Id,
                OwnerId = null
            };

            var uploadResult = await _eventService.UploadAvatar(eventId, file, fileCreateModel);

            if (uploadResult.Succeeded)
            {
                var updatedEvent = uploadResult.Entity;

                if (updatedEvent is null)
                {
                    return Ok(id);
                }

                return Ok(updatedEvent);
            }
            else
            {
                return StatusCode(500, uploadResult.Messages);
            }
        }

        // GET api/Events/EventsCountByDate/?startDateFrom=value1&startDateTo=value2
        [HttpGet("EventsCountByDate")]
        [AllowAnonymous]
        public async Task<ActionResult<IDictionary<DateTime, int>>> GetEventsCountByDate([FromQuery] IDictionary<string, string> dates)
        {
            var eventsCountByDateResult = await _eventService.GetEventsCountByDateAsync(dates);

            if (eventsCountByDateResult.Succeeded)
            {
                return Ok(eventsCountByDateResult.Entity);
            }

            return StatusCode(500, eventsCountByDateResult.Messages);
        }
    }
}
