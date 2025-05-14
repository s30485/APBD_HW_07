using Microsoft.Data.SqlClient;
using APBD_HW_07.Domain.Models;

namespace APBD_HW_07.Business.Interfaces
{
    public interface ICreateCommandBuilder
    {
        bool CanHandle(string prefix);
        SqlCommand Build(SqlConnection conn, DeviceDto device);
    }
}