namespace APBD_HW_07.Data;

public interface IDeviceQueryHandler<TQuery, TResult>
{
    Task<TResult> HandleAsync(TQuery query);
}
