using System.Collections.Generic;
using APBD_HW_07.Domain.Models;

namespace APBD_HW_07.Business.Interfaces
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