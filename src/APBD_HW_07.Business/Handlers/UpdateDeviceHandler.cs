namespace APBD_HW_07.Data;

public class UpdateDeviceHandler : IDeviceCommandHandler<UpdateDeviceCommand>
{
    private readonly IDeviceRepository _repo;

    public UpdateDeviceHandler(IDeviceRepository repo)
    {
        _repo = repo;
    }

    public async Task HandleAsync(UpdateDeviceCommand command)
    {
        var updated = await _repo.UpdateAsync(command.Id, command.Device);
        if (!updated)
            throw new InvalidOperationException("Update failed due to version conflict or invalid ID.");
    }
}
