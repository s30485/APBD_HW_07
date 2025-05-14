namespace APBD_HW_07.Business.Interfaces
{
    public interface IDeviceQueryHandler<in TQuery, TResult>
    {
        Task<TResult> HandleAsync(TQuery query);
    }
}