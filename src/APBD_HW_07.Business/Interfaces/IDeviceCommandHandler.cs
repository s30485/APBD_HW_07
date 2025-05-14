namespace APBD_HW_07.Business.Interfaces
{
    public interface IDeviceCommandHandler<in TCommand>
    {
        Task HandleAsync(TCommand command);
    }
}