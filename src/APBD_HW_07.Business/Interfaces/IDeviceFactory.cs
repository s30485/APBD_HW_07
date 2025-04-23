using APBD_HW_07.Domain;

namespace APBD_HW_07.Business
{
    public interface IDeviceFactory
    {
        Device? CreateFromCsv(string line);
    }
}