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
            var id = $"{dto.Type}-{Guid.NewGuid():N}".Substring(0, 8);
            var d  = new DeviceDto(
                id, dto.Name, dto.IsEnabled,
                dto.BatteryPercentage,
                dto.OperatingSystem,
                dto.IpAddress,
                dto.NetworkName
            );
            await _repo.CreateAsync(d);
            return (await _repo.GetByIdAsync(id))!;
        }

        public Task<bool> UpdateAsync(string id, CreateUpdateDeviceDto dto)
        {
            var d = new DeviceDto(
                id, dto.Name, dto.IsEnabled,
                dto.BatteryPercentage,
                dto.OperatingSystem,
                dto.IpAddress,
                dto.NetworkName
            );
            return _repo.UpdateAsync(id, d);
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

            bool isEnabled = bool.Parse(parts[2]);
            DeviceDto dto = type switch
            {
                "SW" => new DeviceDto(rawId, parts[1], isEnabled,
                                     int.Parse(parts[3].TrimEnd('%')),
                                     null, null, null),
                "P"  => new DeviceDto(rawId, parts[1], isEnabled,
                                     null,
                                     parts.Length > 3 ? parts[3] : string.Empty,
                                     null, null),
                "ED" => new DeviceDto(rawId, parts[1], isEnabled,
                                     null, null,
                                     parts[2], parts[3]),
                _    => throw new ArgumentException("Unreachable")
            };

            await _repo.CreateAsync(dto);
            return (await _repo.GetByIdAsync(rawId))!;
        }
    }
}
