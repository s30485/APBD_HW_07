namespace APBD_HW_07.Business
{
    public interface IDeviceService
    {
        Task<IEnumerable<ShortDeviceDto>> GetAllAsync();
        Task<DeviceDto?> GetByIdAsync(string id);
        Task<DeviceDto> CreateFromJsonAsync(CreateUpdateDeviceDto dto);
        Task<bool> UpdateAsync(string id, CreateUpdateDeviceDto dto);
        Task<bool> DeleteAsync(string id);
        Task<DeviceDto> CreateFromFileLineAsync(string line);
    }
}