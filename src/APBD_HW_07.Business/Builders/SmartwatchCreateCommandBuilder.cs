using APBD_HW_07.Business.Interfaces;
using APBD_HW_07.Domain.Models;
using Microsoft.Data.SqlClient;

namespace APBD_HW_07.Business.Builders
{
    public class SmartwatchCreateCommandBuilder : ICreateCommandBuilder
    {
        public bool CanHandle(string prefix) => prefix == "SW";

        public SqlCommand Build(SqlConnection conn, DeviceDto device)
        {
            var cmd = new SqlCommand("AddSmartwatch", conn)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@DeviceId", device.Id);
            cmd.Parameters.AddWithValue("@Name", device.Name);
            cmd.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);
            cmd.Parameters.AddWithValue("@BatteryPercentage", device.BatteryPercentage 
                                                              ?? throw new ArgumentException("Battery is required for SW"));
            return cmd;
        }
    }
}