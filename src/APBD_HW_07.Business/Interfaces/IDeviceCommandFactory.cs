using Microsoft.Data.SqlClient;
using APBD_HW_07.Domain.Models;

namespace APBD_HW_07.Business.Interfaces
{
    public interface IDeviceCommandFactory
    {
        SqlCommand BuildCreateCommand(SqlConnection conn, DeviceDto device);
    }
}