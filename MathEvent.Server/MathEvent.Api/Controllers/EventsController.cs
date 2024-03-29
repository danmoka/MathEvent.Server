﻿using AutoMapper;
using MathEvent.AuthorizationHandlers;
using MathEvent.Contracts.Services;
using MathEvent.Contracts.Validators;
using MathEvent.DTOs.Events;
using MathEvent.Models.Events;
using MathEvent.Models.Files;
using MathEvent.Models.Others;
using MathEvent.Models.Validation;
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

        private readonly IEventService _eventService;

        private readonly IUserService _userService;

        private readonly IAuthorizationService _authorizationService;

        private readonly IEventCreateModelValidator _eventCreateModelValidator;

        private readonly IEventUpdateModelValidator _eventUpdateModelValidator;

        private readonly IImageValidator _imageValidator;

        public EventsController(
            IMapper mapper,
            IEventService eventService,
            IUserService userService,
            IAuthorizationService authorizationService,
            IEventCreateModelValidator eventCreateModelValidator,
            IEventUpdateModelValidator eventUpdateModelValidator,
            IImageValidator imageValidator)
        {
            _mapper = mapper;
            _eventService = eventService;
            _userService = userService;
            _authorizationService = authorizationService;
            _eventCreateModelValidator = eventCreateModelValidator;
            _eventUpdateModelValidator = eventUpdateModelValidator;
            _imageValidator = imageValidator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventReadModel>>> List([FromQuery] IDictionary<string, string> filters)
        {
            var events = await _eventService.List(filters);

            return Ok(events);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<EventWithUsersReadModel>> Retrieve([FromRoute] int id)
        {
            if (id < 0)
            {
                return BadRequest(new List<ValidationError>()
                {
                    new ValidationError()
                    {
                        Field = "id",
                        Message = $"id = {id} меньше 0"
                    }
                });
            }

            var ev = await _eventService.Retrieve(id);

            if (ev is null)
            {
                return NotFound($"Событие с id={id} не найдено");
            }

            return Ok(ev);
        }

        [HttpPost]
        public async Task<ActionResult<EventWithUsersReadModel>> Create([FromBody] EventCreateModel eventCreateModel)
        {
            var user = await _userService.GetUserByClaims(User);

            if (user is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Не удается получить данные о текущем пользователе");
            }

            eventCreateModel.AuthorId = user.Id;

            var validationResult = await _eventCreateModelValidator.Validate(eventCreateModel);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var createdEvent = await _eventService.Create(eventCreateModel);

            if (createdEvent is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка во время создания события");
            }

            return CreatedAtAction(nameof(Retrieve), new { id = createdEvent.Id }, createdEvent);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] EventUpdateModel eventUpdateModel)
        {
            if (id < 0)
            {
                return BadRequest(new List<ValidationError>()
                    {
                        new ValidationError()
                        {
                            Field = "id",
                            Message = $"id = {id} меньше 0"
                        }
                    });
            }

            var validationResult = await _eventUpdateModelValidator.Validate(eventUpdateModel);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var ev = await _eventService.Retrieve(id);

            if (ev is null)
            {
                return NotFound($"Событие с id={id} не найдено");
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, ev, Operations.Update);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"Вам нельзя редактировать событие с id={id}");
            }

            var updatedEvent = await _eventService.Update(id, eventUpdateModel);

            if (updatedEvent is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка во время обновления события");
            }

            return Ok(updatedEvent);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialUpdate([FromRoute] int id, [FromBody] JsonPatchDocument<EventUpdateModel> patchDocument)
        {
            if (id < 0)
            {
                return BadRequest(new List<ValidationError>()
                {
                    new ValidationError()
                    {
                        Field = "id",
                        Message = $"id = {id} меньше 0"
                    }
                });
            }

            if (patchDocument is null)
            {
                return BadRequest(new List<ValidationError>()
                {
                    new ValidationError()
                    {
                        Field = "body",
                        Message = "Тело запроса не задано"
                    }
                });
            }

            var ev = await _eventService.Retrieve(id);

            if (ev is null)
            {
                return NotFound($"Событие с id={id} не найдено");
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, ev, Operations.Update);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"Вам нельзя редактировать событие с id={id}");
            }

            var eventDTO = _mapper.Map<EventWithUsersDTO>(ev);
            var eventToPatch = _mapper.Map<EventUpdateModel>(eventDTO);
            patchDocument.ApplyTo(eventToPatch, ModelState);

            var validationResult = await _eventUpdateModelValidator.Validate(eventToPatch);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var updatedEvent = await _eventService.Update(id, eventToPatch);

            if (updatedEvent is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка во время обновления события");
            }

            return Ok(updatedEvent);
        }

        [HttpPost("Subscribe/{id}")]
        public async Task<ActionResult> Subscribe([FromRoute] int id)
        {
            if (id < 0)
            {
                return BadRequest(new List<ValidationError>() { new ValidationError() { Field = "id", Message = $"id = {id} меньше 0" } });
            }

            var ev = await _eventService.Retrieve(id);

            if (ev is null)
            {
                return NotFound($"Событие с id={id} не найдено");
            }

            var user = await _userService.GetUserByClaims(User);

            if (user is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Не удается получить данные о текущем пользователе");
            }

            var eventDTO = _mapper.Map<EventWithUsersDTO>(ev);
            var eventToPatch = _mapper.Map<EventUpdateModel>(eventDTO);

            if (!eventToPatch.ApplicationUsers.Contains(user.Id))
            {
                eventToPatch.ApplicationUsers.Add(user.Id);
            }

            var updatedEvent = await _eventService.Update(id, eventToPatch);

            if (updatedEvent is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка во время обновления события");
            }

            return Ok(updatedEvent);
        }

        [HttpPost("Unsubscribe/{id}")]
        public async Task<ActionResult> Unsubscribe([FromRoute] int id)
        {
            if (id < 0)
            {
                return BadRequest(new List<ValidationError>() { new ValidationError() { Field = "id", Message = $"id = {id} меньше 0" } });
            }

            var ev = await _eventService.Retrieve(id);

            if (ev is null)
            {
                return NotFound($"Событие с id={id} не найдено");
            }

            var user = await _userService.GetUserByClaims(User);

            if (user is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Не удается получить данные о текущем пользователе");
            }

            var eventDTO = _mapper.Map<EventWithUsersDTO>(ev);
            var eventToPatch = _mapper.Map<EventUpdateModel>(eventDTO);

            if (eventToPatch.ApplicationUsers.Contains(user.Id))
            {
                eventToPatch.ApplicationUsers.Remove(user.Id);
            }

            var updatedEvent = await _eventService.Update(id, eventToPatch);

            if (updatedEvent is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка во время обновления события");
            }

            return Ok(updatedEvent);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Destroy([FromRoute] int id)
        {
            if (id < 0)
            {
                return BadRequest(new List<ValidationError>() { new ValidationError() { Field = "id", Message = $"id = {id} меньше 0" } });
            }

            var ev = await _eventService.Retrieve(id);

            if (ev is null)
            {
                return NotFound($"Событие с id={id} не найдено");
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, ev, Operations.Delete);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"Вам нельзя удалить событие с id={id}");
            }

            var childEvents = new List<EventReadModel>(await _eventService.GetChildEvents(id));

            if (childEvents.Count > 0)
            {
                return BadRequest(new List<ValidationError>()
                {
                    new ValidationError()
                    {
                        Field = "id",
                        Message = $"Событие с id = {id} имеет дочерние события"
                    }
                });
            }

            await _eventService.Delete(id);

            return NoContent();
        }

        [HttpGet("Breadcrumbs/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Breadcrumb>>> GetBreadcrumbs([FromRoute] int id)
        {
            if (id < 0)
            {
                return BadRequest(new List<ValidationError>() { new ValidationError() { Field = "id", Message = $"id = {id} меньше 0" } });
            }

            var breadcrumbs = await _eventService.GetBreadcrumbs(id);

            return Ok(breadcrumbs);
        }

        [HttpGet("Statistics/")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ChartData>>> Statistics([FromQuery] IDictionary<string, string> filters)
        {
            var eventsStatistics = await _eventService.GetEventsStatistics(filters);

            return Ok(eventsStatistics);
        }

        [HttpGet("Statistics/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ChartData>>> EventStatistics([FromRoute] int id)
        {
            var eventStatistics = await _eventService.GetEventStatistics(id);

            return Ok(eventStatistics);
        }

        [HttpPost("Avatar")]
        public async Task<ActionResult> UploadAvatar([FromForm] IFormFile file, [FromQuery] string id)
        {
            var eventId = -1;

            if (int.TryParse(id, out int eventIdParam))
            {
                eventId = eventIdParam;
            }

            if (eventId < 0)
            {
                return BadRequest(new List<ValidationError>() { new ValidationError() { Field = "id", Message = $"id = {id} меньше 0" } });
            }

            var ev = await _eventService.Retrieve(eventId);

            if (ev is null)
            {
                return NotFound($"Событие с id={id} не найдено");
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, ev, Operations.Update);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"Вам нельзя редактировать событие с id={id}");
            }

            var checkResult = await _imageValidator.Validate(file);

            if (!checkResult.IsValid)
            {
                return BadRequest(checkResult.Errors);
            }

            var user = await _userService.GetUserByClaims(User);

            if (user is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Не удается определить текущего пользователя");
            }

            var fileCreateModel = new FileCreateModel
            {
                Name = Path.GetFileNameWithoutExtension(file.FileName),
                Hierarchy = null,
                ParentId = null,
                AuthorId = user.Id,
                OwnerId = null
            };

            var updatedEvent = await _eventService.UploadAvatar(eventId, file, fileCreateModel);

            if (updatedEvent is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка по время загрузки изображения");
            }

            return Ok(updatedEvent);
        }

        [HttpGet("EventsCountByDate")]
        [AllowAnonymous]
        public async Task<ActionResult<IDictionary<DateTime, int>>> GetEventsCountByDate([FromQuery] IDictionary<string, string> dates)
        {
            var eventsCountByDate = await _eventService.GetEventsCountByDateAsync(dates);

            return Ok(eventsCountByDate);
        }

        [HttpGet("SortByValues")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<SortDataModel>> GetSortByValues()
        {
            return Ok(_eventService.GetSortByValues());
        }
    }
}
