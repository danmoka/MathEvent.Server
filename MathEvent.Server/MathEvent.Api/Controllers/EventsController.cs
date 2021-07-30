using AutoMapper;
using MathEvent.Converters.Events.DTOs;
using MathEvent.Converters.Events.Models;
using MathEvent.Converters.Files.Models;
using MathEvent.Converters.Others;
using MathEvent.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
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

        private readonly IUserService _userService;

        public EventsController(IMapper mapper, EventService eventService, FileService fileService, IUserService userService)
        {
            _mapper = mapper;
            _eventService = eventService;
            _fileService = fileService;
            _userService = userService;
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
                return BadRequest();
            }

            var eventResult = await _eventService.RetrieveAsync(id);

            if (eventResult.Succeeded)
            {
                var eventReadModel = eventResult.Entity;

                if (eventReadModel is not null)
                {
                    return Ok(eventReadModel);
                }
            }

            return NotFound(eventResult.Messages);
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
                    return StatusCode(201);
                }

                return StatusCode(201, createdEvent);
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

            var eventResult = await _eventService.GetEventEntityAsync(id);

            if (!eventResult.Succeeded)
            {
                return NotFound(eventResult.Messages);
            }

            if (!TryValidateModel(eventUpdateModel))
            {
                return ValidationProblem(ModelState);
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

            var eventResult = await _eventService.GetEventEntityAsync(id);

            if (!eventResult.Succeeded)
            {
                return NotFound(eventResult.Messages);
            }

            var eventEntity = eventResult.Entity;

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
                var updatedEvent = updateResult.Entity;

                if (updatedEvent is null)
                {
                    return Ok(id);
                }

                return Ok(updatedEvent);
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

            var eventResult = await _eventService.GetEventEntityAsync(id);

            if (!eventResult.Succeeded)
            {
                return NotFound(eventResult.Messages);
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

        // GET api/Events/Statistics/?key1=value1&key2=value2
        [HttpGet("Statistics/")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SimpleStatistics>>> StatisticsAsync([FromQuery] IDictionary<string, string> filters)
        {
            var eventsStatisticsResult = await _eventService.GetSimpleStatistics(filters);

            if (eventsStatisticsResult.Succeeded)
            {
                if (eventsStatisticsResult.Entity is not null)
                {
                    return Ok(eventsStatisticsResult.Entity);
                }
            }

            return BadRequest(eventsStatisticsResult.Messages);
        }

        // GET api/Events/Statistics/{id}
        [HttpGet("Statistics/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SimpleStatistics>>> EventStatisticsAsync(int id)
        {
            var eventStatisticsResult = await _eventService.GetEventStatistics(id);

            if (eventStatisticsResult.Succeeded)
            {
                if (eventStatisticsResult.Entity is not null)
                {
                    return Ok(eventStatisticsResult.Entity);
                }
            }

            return BadRequest(eventStatisticsResult.Messages);
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
                return BadRequest();
            }

            var eventResult = await _eventService.GetEventEntityAsync(eventId);

            if (!eventResult.Succeeded)
            {
                return NotFound(eventResult.Messages);
            }

            var checkResult = _fileService.IsCorrectImage(file);

            if (!checkResult.Succeeded)
            {
                return BadRequest(checkResult.Messages);
            }

            var user = await _userService.GetCurrentUserAsync(User);

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
                return BadRequest(uploadResult.Messages);
            }
        }
    }
}
