using MathEvent.AuthorizationHandlers;
using MathEvent.Contracts.Services;
using MathEvent.Contracts.Validators;
using MathEvent.Models.Files;
using MathEvent.Models.Others;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MathEvent.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        private readonly IUserService _userService;

        private readonly IAuthorizationService _authorizationService;

        private readonly IFileCreateModelValidator _fileCreateModelValidator;

        private readonly IFileUpdateModelValidator _fileUpdateModelValidator;

        private readonly IFileValidator _fileValidator;

        public FilesController(
            IFileService fileService,
            IUserService userService,
            IAuthorizationService authorizationService,
            IFileCreateModelValidator fileCreateModelValidator,
            IFileUpdateModelValidator fileUpdateModelValidator,
            IFileValidator fileValidator)
        {
            _fileService = fileService;
            _userService = userService;
            _authorizationService = authorizationService;
            _fileCreateModelValidator = fileCreateModelValidator;
            _fileUpdateModelValidator = fileUpdateModelValidator;
            _fileValidator = fileValidator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FileReadModel>>> List([FromQuery] IDictionary<string, string> filters)
        {
            var files = await _fileService.ListAsync(filters);

            return Ok(files);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FileReadModel>> Retrieve([FromRoute] int id)
        {
            if (id < 0)
            {
                return BadRequest($"id={id} меньше 0");
            }

            var file = await _fileService.RetrieveAsync(id);

            if (file is null)
            {
                return NotFound($"Файл с id={id} не найден");
            }

            return Ok(file);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] FileCreateModel fileCreateModel)
        {
            var validationResult = await _fileCreateModelValidator.Validate(fileCreateModel);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, fileCreateModel.OwnerId, Operations.Create);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"Вам нельзя создавать файл для объекта-владельца с id={fileCreateModel.OwnerId}");
            }

            var user = await _userService.GetUserAsync(User);

            if (user is null)
            {
                return NotFound("Не удается определить текущего пользователя");
            }

            fileCreateModel.AuthorId = user.Id;
            var createdFile = await _fileService.CreateAsync(fileCreateModel);

            if (createdFile is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка во время создания файла");
            }

            return CreatedAtAction(nameof(Retrieve), new { id = createdFile.Id }, createdFile);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] FileUpdateModel fileUpdateModel)
        {
            if (id < 0)
            {
                return BadRequest($"id={id} меньше 0");
            }

            var validationResult = await _fileUpdateModelValidator.Validate(fileUpdateModel);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var file = await _fileService.RetrieveAsync(id);

            if (file is null)
            {
                return NotFound($"Файл с id={id} не найден");
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, file.OwnerId, Operations.Update);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"Вам нельзя редактировать файл для объекта-владельца с id={file.OwnerId}");
            }

            var updatedFile = await _fileService.UpdateAsync(id, fileUpdateModel);

            if (updatedFile is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка во время обновления файла");
            }

            return Ok(updatedFile);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Destroy([FromRoute] int id)
        {
            if (id < 0)
            {
                return BadRequest($"id={id} меньше 0");
            }

            var file = await _fileService.RetrieveAsync(id);

            if (file is null)
            {
                return NotFound($"Файл с id={id} не найден");
            }

            var child = new List<FileReadModel>(await _fileService.GetChildFiles(id));

            if (child.Count > 0)
            {
                return BadRequest($"Файл с id={id} имеет дочерние файлы");
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, file.OwnerId, Operations.Delete);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"Вам нельзя удалять файл объекта-владельца с id={file.OwnerId}");
            }

            await _fileService.DeleteAsync(id);

            return NoContent();
        }

        [HttpPost("Upload")]
        public async Task<ActionResult> Upload([FromForm] IEnumerable<IFormFile> files, [FromQuery] string parentId, [FromQuery] string ownerId)
        {
            int? parent = null;
            int? owner = null;

            if (int.TryParse(parentId, out int parentParam))
            {
                parent = parentParam;
            }

            if (int.TryParse(ownerId, out int ownerParam))
            {
                owner = ownerParam;
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, owner, Operations.Create);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"Вам нельзя загружать файл для объекта-владельца с id={owner}");
            }


            foreach (var formFile in files)
            {
                var checkResult = await _fileValidator.Validate(formFile);

                if (!checkResult.IsValid)
                {
                    return BadRequest(checkResult.Errors);
                }
            }

            var user = await _userService.GetUserAsync(User);

            if (user is null)
            {
                return NotFound("Не удается определить текущего пользователя");
            }

            var fileIds = new List<int>();

            foreach (var formFile in files)
            {
                var fileCreateModel = new FileCreateModel
                {
                    Name = Path.GetFileNameWithoutExtension(formFile.FileName),
                    Hierarchy = null,
                    ParentId = parent,
                    AuthorId = user.Id,
                    OwnerId = owner
                };

                var validationResult = await _fileCreateModelValidator.Validate(fileCreateModel);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var createdFile = await _fileService.Upload(formFile, fileCreateModel);

                if (createdFile is null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка во время загрузки изображения");
                }

                fileIds.Add(createdFile.Id);
            }

            return Ok(fileIds);
        }

        [HttpGet("Download/{id}")]
        public async Task<ActionResult> Download([FromRoute] int id)
        {
            if (id < 0)
            {
                return BadRequest($"id={id} меньше 0");
            }

            var file = await _fileService.RetrieveAsync(id);

            if (file is null)
            {
                return NotFound($"Файл с id={id} не найден");
            }

            if (file.Hierarchy is not null)
            {
                return BadRequest("Скачивание папок невозможно");
            }

            var downloadedFile = await _fileService.Download(id);

            if (downloadedFile is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка во время получения файла для скачивания");
            }

            return File(downloadedFile, "application/octet-stream", $"\"{HttpUtility.UrlEncode(file.Name, Encoding.UTF8)}{file.Extension}\"");
        }

        [HttpGet("Breadcrumbs/{id}")]
        public async Task<ActionResult<IEnumerable<Breadcrumb>>> GetBreadcrumbs([FromRoute] int id)
        {
            if (id < 0)
            {
                return BadRequest($"id={id} меньше 0");
            }

            var breadcrumbs = await _fileService.GetBreadcrumbs(id);

            return Ok(breadcrumbs);
        }
    }
}
