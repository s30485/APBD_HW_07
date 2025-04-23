namespace APBD_HW_07.Business
{
    public record ShortDeviceDto(string Id, string Name, bool IsEnabled);
    public record DeviceDto(
        string Id,
        string Name,
        bool IsEnabled,
        int? BatteryPercentage,
        string? OperatingSystem,
        string? IpAddress,
        string? NetworkName
    );
    public record CreateUpdateDeviceDto(
        string Type,
        string Name,
        bool IsEnabled,
        int? BatteryPercentage,
        string? OperatingSystem,
        string? IpAddress,
        string? NetworkName
    );
}