using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using APBD_HW_07.Business;
using APBD_HW_07.Domain.Exceptions;
using APBD_HW_07.RestAPI.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace APBD_HW_07.RestAPI.Controllers
{
    [ApiController]
    [Route("api/devices")]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _svc;
        private readonly IValidator<CreateUpdateDeviceDto> _validator;

        public DevicesController(
            IDeviceService svc,
            IValidator<CreateUpdateDeviceDto> validator)
        {
            _svc = svc;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _svc.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var dto = await _svc.GetByIdAsync(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpPost("{deviceType}")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateJson(
            string deviceType,
            [FromBody] CreateUpdateDeviceDto dto)
        {
            // override Type from route
            dto = dto with { Type = deviceType };

            // 1) validate DTO + domain invariants
            var (ok, errors) = _validator.Validate(dto);
            if (!ok)
                return BadRequest(new { errors });

            // 2) create
            try
            {
                var created = await _svc.CreateFromJsonAsync(dto);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.Id },
                    created);
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
            catch (EmptyBatteryException ebe)
            {
                return BadRequest(ebe.Message);
            }
            catch (EmptySystemException ese)
            {
                return BadRequest(ese.Message);
            }
            catch (ConnectionException ce)
            {
                return BadRequest(ce.Message);
            }
        }

        [HttpPost]
        [Consumes("text/plain")]
        public async Task<IActionResult> ImportPlainText([FromBody] string body)
        {
            var created = new List<DeviceDto>();
            using var reader = new StringReader(body);
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                try
                {
                    // Service parses, validates, and persists
                    var dto = await _svc.CreateFromFileLineAsync(line);
                    created.Add(dto);
                }
                catch
                {
                    // skip invalid lines
                }
            }
            return Ok(created);
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        public async Task<IActionResult> Update(
            string id,
            [FromBody] CreateUpdateDeviceDto dto)
        {
            // DTO‐level override
            // Actually deviceType isn’t needed for update—ignored.

            // 1) validate fields
            var (ok, errors) = _validator.Validate(dto);
            if (!ok)
                return BadRequest(new { errors });

            // 2) update
            var updated = await _svc.UpdateAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
            => await _svc.DeleteAsync(id)
                ? NoContent()
                : NotFound();
    }
}
