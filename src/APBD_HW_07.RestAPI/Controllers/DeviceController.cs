using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using APBD_HW_07.Business;
using APBD_HW_07.Business.Interfaces;
using APBD_HW_07.Domain.Exceptions;
using APBD_HW_07.RestAPI.Validators;
using Microsoft.AspNetCore.Mvc;

namespace APBD_HW_07.RestAPI.Controllers;

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
    public async Task<IResult> GetAll()
    {
        var list = await _svc.GetAllAsync();
        return Results.Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IResult> GetById(string id)
    {
        var dto = await _svc.GetByIdAsync(id);
        return dto is null ? Results.NotFound() : Results.Ok(dto);
    }

    [HttpPost("{deviceType}")]
    [Consumes("application/json")]
    public async Task<IResult> CreateJson(string deviceType, [FromBody] CreateUpdateDeviceDto dto)
    {
        dto = dto with { Type = deviceType };

        var (ok, errors) = _validator.Validate(dto);
        if (!ok) return Results.BadRequest(new { errors });

        try
        {
            var created = await _svc.CreateFromJsonAsync(dto);
            return Results.Created($"/api/devices/{created.Id}", created);
        }
        catch (ArgumentException ae)
        {
            return Results.BadRequest(ae.Message);
        }
        catch (EmptyBatteryException ebe)
        {
            return Results.BadRequest(ebe.Message);
        }
        catch (EmptySystemException ese)
        {
            return Results.BadRequest(ese.Message);
        }
        catch (ConnectionException ce)
        {
            return Results.BadRequest(ce.Message);
        }
    }

    [HttpPut("{id}")]
    [Consumes("application/json")]
    public async Task<IResult> Update(string id, [FromBody] CreateUpdateDeviceDto dto)
    {
        var (ok, errors) = _validator.Validate(dto);
        if (!ok) return Results.BadRequest(new { errors });

        var updated = await _svc.UpdateAsync(id, dto);
        return updated ? Results.NoContent() : Results.Conflict("Update failed due to version conflict.");
    }

    [HttpDelete("{id}")]
    public async Task<IResult> Delete(string id)
    {
        var deleted = await _svc.DeleteAsync(id);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}
