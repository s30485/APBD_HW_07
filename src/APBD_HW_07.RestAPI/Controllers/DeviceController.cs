using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using APBD_HW_07.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace APBD_HW_07.RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _svc;

        public DeviceController(IDeviceService svc)
        {
            _svc = svc;
        }

        /// <summary>
        /// GET /api/devices
        /// Returns short info (ID, Name, IsEnabled).
        /// </summary>
        [HttpGet]
        public async Task<IResult> GetAll()
            => Results.Ok(await _svc.GetAllAsync());

        /// <summary>
        /// GET /api/devices/{id}
        /// Returns full details for one device.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IResult> GetById(string id)
        {
            var dto = await _svc.GetByIdAsync(id);
            return dto is null
                ? Results.NotFound()
                : Results.Ok(dto);
        }

        /// <summary>
        /// POST /api/devices/{deviceType}
        /// Creates a new device of the given type from JSON.
        /// ID is generated server-side.
        /// </summary>
        [HttpPost("{deviceType}")]
        [Consumes("application/json")]
        public async Task<IResult> Create(string deviceType, [FromBody] CreateUpdateDeviceDto dto)
        {
            var payload = dto with { Type = deviceType };
            var created = await _svc.CreateFromJsonAsync(payload);
            return Results.Created($"/api/devices/{created.Id}", created);
        }

        /// <summary>
        /// POST /api/devices
        /// Bulk-import from plain-text CSV-style lines.
        /// </summary>
        [HttpPost]
        [Consumes("text/plain")]
        public async Task<IResult> Import([FromBody] string csvText)
        {
            var created = new List<DeviceDto>();
            using var reader = new StringReader(csvText);
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                try
                {
                    var dto = await _svc.CreateFromFileLineAsync(line);
                    created.Add(dto);
                }
                catch
                {
                    // skip invalid lines
                }
            }
            return Results.Ok(created);
        }

        /// <summary>
        /// PUT /api/devices/{id}
        /// Updates an existing device by ID.
        /// </summary>
        [HttpPut("{id}")]
        [Consumes("application/json")]
        public async Task<IResult> Update(string id, [FromBody] CreateUpdateDeviceDto dto)
            => await _svc.UpdateAsync(id, dto)
                ? Results.NoContent()
                : Results.NotFound();

        /// <summary>
        /// DELETE /api/devices/{id}
        /// Deletes the device by ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IResult> Delete(string id)
            => await _svc.DeleteAsync(id)
                ? Results.NoContent()
                : Results.NotFound();
    }
}
