namespace APBD_HW_07.Data;

public class GetDeviceByIdHandler : IDeviceQueryHandler<GetDeviceByIdQuery, DeviceDto?>
{
    private readonly IDeviceRepository _repo;

    public GetDeviceByIdHandler(IDeviceRepository repo)
    {
        _repo = repo;
    }

    public Task<DeviceDto?> HandleAsync(GetDeviceByIdQuery query)
    {
        return _repo.GetByIdAsync(query.Id);
    }
}
