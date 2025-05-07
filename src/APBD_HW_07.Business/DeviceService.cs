using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APBD_HW_07.Domain.Models;

namespace APBD_HW_07.Business
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _repo;

        public DeviceService(IDeviceRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<ShortDeviceDto>> GetAllAsync()
            => _repo.GetAllAsync();

        public Task<DeviceDto?> GetByIdAsync(string id)
            => _repo.GetByIdAsync(id);

        public async Task<DeviceDto> CreateFromJsonAsync(CreateUpdateDeviceDto dto)
        {
            var prefix = dto.Type.ToUpperInvariant();

            //1. generate next ID based on existing IDs
            var all = await _repo.GetAllAsync();
            var maxSuffix = all
                .Select(d => d.Id)
                .Where(id => id.StartsWith(prefix + "-"))
                .Select(id =>
                {
                    var parts = id.Split('-');
                    return parts.Length == 2 && int.TryParse(parts[1], out var n) ? n : 0;
                })
                .DefaultIfEmpty(0)
                .Max();

            var newId = $"{prefix}-{maxSuffix + 1}";

            //2. make new DeviceDto
            var device = new DeviceDto(
                newId,
                dto.Name,
                dto.IsEnabled,
                dto.BatteryPercentage,
                dto.OperatingSystem,
                dto.IpAddress,
                dto.NetworkName,
                Array.Empty<byte>() // dummy RowVersion for creation; DB sets it based on the procedure
            );

            //3. use repository (which uses stored procedure + transaction)
            await _repo.CreateAsync(device);

            //4. return created (with real RowVersion)
            return (await _repo.GetByIdAsync(newId))!;
        }

        public async Task<bool> UpdateAsync(string id, CreateUpdateDeviceDto dto)
        {
            var device = new DeviceDto(
                id,
                dto.Name,
                dto.IsEnabled,
                dto.BatteryPercentage,
                dto.OperatingSystem,
                dto.IpAddress,
                dto.NetworkName,
                dto.RowVersion //needed for optimistic locking
            );

            return await _repo.UpdateAsync(id, device);
        }

        public Task<bool> DeleteAsync(string id)
            => _repo.DeleteAsync(id);

        public async Task<DeviceDto> CreateFromFileLineAsync(string line)
        {
            var parts = line.Split(',');
            if (parts.Length < 3)
                throw new ArgumentException("Line must have at least 3 comma-separated parts.");

            var rawId = parts[0].Trim();
            if (string.IsNullOrWhiteSpace(rawId))
                throw new ArgumentException("Missing device ID.");

            var type = rawId.StartsWith("SW-") ? "SW"
                     : rawId.StartsWith("P-")  ? "P"
                     : rawId.StartsWith("ED-") ? "ED"
                     : throw new ArgumentException($"Unknown prefix in ID '{rawId}'.");

            var name = parts[1].Trim();
            var isEnabled = int.Parse(parts[2]);

            DeviceDto dto = type switch
            {
                "SW" => new DeviceDto(
                            rawId,
                            name,
                            isEnabled,
                            int.Parse(parts[3].TrimEnd('%')),
                            null, null, null,
                            Array.Empty<byte>()
                        ),
                "P" => new DeviceDto(
                           rawId,
                           name,
                           isEnabled,
                           null,
                           parts.Length > 3 ? parts[3].Trim() : string.Empty,
                           null, null,
                           Array.Empty<byte>()
                       ),
                "ED" => new DeviceDto(
                            rawId,
                            name,
                            isEnabled,
                            null, null,
                            parts.Length > 3 ? parts[3].Trim() : string.Empty,
                            parts.Length > 4 ? parts[4].Trim() : string.Empty,
                            Array.Empty<byte>()
                        ),
                _ => throw new ArgumentException("Unreachable device type.")
            };

            await _repo.CreateAsync(dto);
            return (await _repo.GetByIdAsync(rawId))!;
        }
    }
}
