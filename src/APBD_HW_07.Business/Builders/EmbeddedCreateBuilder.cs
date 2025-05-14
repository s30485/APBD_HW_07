using APBD_HW_07.Business.Interfaces;
using APBD_HW_07.Domain.Models;
using Microsoft.Data.SqlClient;

namespace APBD_HW_07.Business.Builders
{
    public class EmbeddedCreateCommandBuilder : ICreateCommandBuilder
    {
        public bool CanHandle(string prefix) => prefix == "ED";

        public SqlCommand Build(SqlConnection conn, DeviceDto device)
        {
            var cmd = new SqlCommand("AddEmbedded", conn)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@DeviceId", device.Id);
            cmd.Parameters.AddWithValue("@Name", device.Name);
            cmd.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);
            cmd.Parameters.AddWithValue("@IpAddress", device.IpAddress 
                                                      ?? throw new ArgumentException("IP is required for ED"));
            cmd.Parameters.AddWithValue("@NetworkName", device.NetworkName 
                                                        ?? throw new ArgumentException("Network is required for ED"));
            return cmd;
        }
    }
}