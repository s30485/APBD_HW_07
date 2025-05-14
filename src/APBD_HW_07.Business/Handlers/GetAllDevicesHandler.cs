namespace APBD_HW_07.Data;

public class GetAllDevicesHandler : IDeviceQueryHandler<GetAllDevicesQuery, IEnumerable<ShortDeviceDto>>
{
    private readonly IDeviceRepository _repo;

    public GetAllDevicesHandler(IDeviceRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<ShortDeviceDto>> HandleAsync(GetAllDevicesQuery query)
    {
        return _repo.GetAllAsync();
    }
}
