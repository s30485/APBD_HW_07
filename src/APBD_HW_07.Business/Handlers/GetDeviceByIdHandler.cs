using APBD_HW_07.Business.Interfaces;
using APBD_HW_07.Business.Queries;
using APBD_HW_07.Domain.Models;

namespace APBD_HW_07.Business.Handlers
{
    public class GetDeviceByIdHandler 
        : IDeviceQueryHandler<GetDeviceByIdQuery, DeviceDto?>
    {
        private readonly IDeviceRepository _repo;
        public GetDeviceByIdHandler(IDeviceRepository repo) => _repo = repo;

        public Task<DeviceDto?> HandleAsync(GetDeviceByIdQuery query)
            => _repo.GetByIdAsync(query.Id);
    }
}