using APBD_HW_07.Business;
using Microsoft.AspNetCore.Mvc;

namespace APBD_HW_07.RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _svc;
        private readonly IDeviceFileImporter _importer;

        public DevicesController(IDeviceService svc, IDeviceFileImporter importer)
        {
            _svc = svc;
            _importer = importer;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _svc.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var dto = await _svc.GetByIdAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateJson([FromBody] CreateUpdateDeviceDto dto)
        {
            var created = await _svc.CreateFromJsonAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPost("import-file")]
        public async Task<IActionResult> ImportFromFile([FromQuery]string path)
        {
            try
            {
                await _importer.ImportFromFileAsync(path);
                return NoContent();
            }
            catch (FileNotFoundException)
            {
                return NotFound(path);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id,
            [FromBody] CreateUpdateDeviceDto dto)
        {
            return await _svc.UpdateAsync(id, dto) ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
            => await _svc.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
