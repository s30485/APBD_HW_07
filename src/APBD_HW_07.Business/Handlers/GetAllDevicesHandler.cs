using System.Collections.Generic;
using APBD_HW_07.Business.Interfaces;
using APBD_HW_07.Business.Queries;
using APBD_HW_07.Domain.Models;

namespace APBD_HW_07.Business.Handlers
{
    public class GetAllDevicesHandler 
        : IDeviceQueryHandler<GetAllDevicesQuery, IEnumerable<ShortDeviceDto>>
    {
        private readonly IDeviceRepository _repo;
        public GetAllDevicesHandler(IDeviceRepository repo) => _repo = repo;

        public Task<IEnumerable<ShortDeviceDto>> HandleAsync(GetAllDevicesQuery _)
            => _repo.GetAllAsync();
    }
}