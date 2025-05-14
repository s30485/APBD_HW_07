using APBD_HW_07.Business.Interfaces;
using APBD_HW_07.Domain.Models;
using Microsoft.Data.SqlClient;

namespace APBD_HW_07.Business.Builders
{
    public class PersonalComputerCreateCommandBuilder : ICreateCommandBuilder
    {
        public bool CanHandle(string prefix) => prefix == "P-";

        public SqlCommand Build(SqlConnection conn, DeviceDto device)
        {
            var cmd = new SqlCommand("AddPersonalComputer", conn)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@DeviceId", device.Id);
            cmd.Parameters.AddWithValue("@Name", device.Name);
            cmd.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);
            cmd.Parameters.AddWithValue("@OperationSystem", device.OperatingSystem 
                                                            ?? throw new ArgumentException("OS is required for P-"));
            return cmd;
        }
    }
}