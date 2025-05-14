namespace APBD_HW_07.Data;

public class CreateDeviceHandler : IDeviceCommandHandler<CreateDeviceCommand>
{
    private readonly IDeviceRepository _repo;

    public CreateDeviceHandler(IDeviceRepository repo)
    {
        _repo = repo;
    }

    public async Task HandleAsync(CreateDeviceCommand command)
    {
        await _repo.CreateAsync(command.Device);
    }
}
