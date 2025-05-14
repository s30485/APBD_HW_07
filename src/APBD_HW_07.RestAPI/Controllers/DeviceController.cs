using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using APBD_HW_07.Business;
using APBD_HW_07.Business.Commands;
using APBD_HW_07.Business.Queries;
using APBD_HW_07.Business.Interfaces;
using APBD_HW_07.Domain.Exceptions;
using APBD_HW_07.Domain.Models;
using APBD_HW_07.RestAPI.Validators;
using Microsoft.AspNetCore.Mvc;

namespace APBD_HW_07.RestAPI.Controllers
{
    [ApiController]
    [Route("api/devices")]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceQueryHandler<GetAllDevicesQuery, IEnumerable<ShortDeviceDto>> _getAll;
        private readonly IDeviceQueryHandler<GetDeviceByIdQuery, DeviceDto?> _getById;
        private readonly IDeviceCommandHandler<CreateDeviceCommand> _create;
        private readonly IDeviceCommandHandler<UpdateDeviceCommand> _update;
        private readonly IDeviceCommandHandler<DeleteDeviceCommand> _delete;
        private readonly IValidator<CreateUpdateDeviceDto> _validator;

        public DevicesController(
            IDeviceQueryHandler<GetAllDevicesQuery, IEnumerable<ShortDeviceDto>> getAll,
            IDeviceQueryHandler<GetDeviceByIdQuery, DeviceDto?> getById,
            IDeviceCommandHandler<CreateDeviceCommand> create,
            IDeviceCommandHandler<UpdateDeviceCommand> update,
            IDeviceCommandHandler<DeleteDeviceCommand> delete,
            IValidator<CreateUpdateDeviceDto> validator)
        {
            _getAll = getAll;
            _getById = getById;
            _create = create;
            _update = update;
            _delete = delete;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IResult> GetAll()
            => Results.Ok(await _getAll.HandleAsync(new GetAllDevicesQuery()));

        [HttpGet("{id}")]
        public async Task<IResult> GetById(string id)
        {
            var dto = await _getById.HandleAsync(new GetDeviceByIdQuery(id));
            return dto is null ? Results.NotFound() : Results.Ok(dto);
        }

        [HttpPost("{deviceType}")]
        [Consumes("application/json")]
        public async Task<IResult> CreateJson(string deviceType, [FromBody] CreateUpdateDeviceDto dto)
        {
            dto = dto with { Type = deviceType };
            var (ok, errs) = _validator.Validate(dto);
            if (!ok) 
                return Results.BadRequest(new { errs });

            //load only the ids that match the prefix
            var all = await _getAll.HandleAsync(new GetAllDevicesQuery());
            var matches = all
                .Select(d => d.Id)
                .Where(id => id.StartsWith($"{dto.Type}-", StringComparison.OrdinalIgnoreCase));

            //pull out the numeric part of id
            var max = matches
                .Select(id =>
                {
                    var parts = id.Split('-', 2);
                    return parts.Length == 2 && int.TryParse(parts[1], out var n) ? n : 0;
                })
                .DefaultIfEmpty(0)
                .Max();

            //increment
            var next = max + 1;

            //combine
            var newId = $"{dto.Type}-{next}"; //imo a good option would be to make a seperate class that does this studd to follow SOLID better, but this works and i dont have much time.
            
            var device = new DeviceDto(
                newId, 
                dto.Name, 
                dto.IsEnabled,
                dto.BatteryPercentage, 
                dto.OperatingSystem,
                dto.IpAddress, 
                dto.NetworkName,
                null);

            try
            {
                await _create.HandleAsync(new CreateDeviceCommand(device));
                return Results.Created($"/api/devices/{newId}", device);
            }
            catch (Exception ex) when (ex is ArgumentException ||
                                       ex is EmptyBatteryException ||
                                       ex is EmptySystemException ||
                                       ex is ConnectionException)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        public async Task<IResult> Update(string id, [FromBody] CreateUpdateDeviceDto dto)
        {
            var (ok, errs) = _validator.Validate(dto);
            if (!ok) return Results.BadRequest(new { errs });

            var device = new DeviceDto(
                id, dto.Name, dto.IsEnabled,
                dto.BatteryPercentage, dto.OperatingSystem,
                dto.IpAddress, dto.NetworkName,
                dto.RowVersion);

            try
            {
                await _update.HandleAsync(new UpdateDeviceCommand(id, device));
                return Results.NoContent();
            }
            catch (InvalidOperationException)
            {
                return Results.Conflict("Invalid version conflict");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IResult> Delete(string id)
        {
            try
            {
                await _delete.HandleAsync(new DeleteDeviceCommand(id));
                return Results.NoContent();
            }
            catch (InvalidOperationException)
            {
                return Results.NotFound();
            }
        }
    }
}
