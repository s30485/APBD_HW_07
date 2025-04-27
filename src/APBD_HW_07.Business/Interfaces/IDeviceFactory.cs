using APBD_HW_07.Domain;
using APBD_HW_07.Domain.Models;

namespace APBD_HW_07.Business
{
    public interface IDeviceFactory
    {
        Device? CreateFromCsv(string line);
    }
}