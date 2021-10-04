using MathEvent.Services.Services.DataPath;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MathEvent.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        // TODO: заменить на file service
        private readonly DataPathWorker _dataPathService;

        public ImagesController(DataPathWorker dataPathService)
        {
            _dataPathService = dataPathService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetImage([FromQuery] string src)
        {
            // TODO: можно получить любой файл, зная к нему путь
            var image = _dataPathService.GetFileStream(src);

            return Ok(image);
        }
    }
}
