namespace APBD_HW_07.Business
{
    public interface IDeviceRepository
    {
        Task<IEnumerable<ShortDeviceDto>> GetAllAsync();
        Task<DeviceDto?> GetByIdAsync(string id);
        Task CreateAsync(DeviceDto device);
        Task<bool> UpdateAsync(string id, DeviceDto device);
        Task<bool> DeleteAsync(string id);
    }
}