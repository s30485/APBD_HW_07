namespace APBD_HW_07.Data;

public interface IDeviceCommandHandler<TCommand>
{
    Task HandleAsync(TCommand command);
}
