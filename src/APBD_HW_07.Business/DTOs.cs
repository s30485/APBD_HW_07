namespace APBD_HW_07.Business
{
    public record ShortDeviceDto(string Id, string Name, int IsEnabled);
    
    //done like in the lecture
    public record DeviceDto(
        string Id,
        string Name,
        int IsEnabled,
        int? BatteryPercentage,
        string? OperatingSystem,
        string? IpAddress,
        string? NetworkName,
        byte[]? RowVersion //can be nullable when making a device 
    );
    public record CreateUpdateDeviceDto(
        string Type,
        string Name,
        int IsEnabled,
        int? BatteryPercentage,
        string? OperatingSystem,
        string? IpAddress,
        string? NetworkName,
        byte[]? RowVersion
    );
}