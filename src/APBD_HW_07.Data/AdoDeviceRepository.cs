using System.Data;
using APBD_HW_07.Business;
using Microsoft.Data.SqlClient;

namespace APBD_HW_07.Data
{
    public class AdoDeviceRepository : IDeviceRepository
    {
        private readonly string _conn;
        public AdoDeviceRepository(string connectionString)
        {
            _conn = connectionString;
        }

        public async Task<IEnumerable<ShortDeviceDto>> GetAllAsync()
        {
            var list = new List<ShortDeviceDto>();
            const string sql = @"SELECT Id, Name, IsEnabled FROM Devices";

            using var conn = new SqlConnection(_conn);
            using var cmd  = new SqlCommand(sql, conn);
            await conn.OpenAsync();
            using var rdr = await cmd.ExecuteReaderAsync();

            while (await rdr.ReadAsync())
                list.Add(new ShortDeviceDto(
                    rdr.GetString(0),
                    rdr.GetString(1),
                    rdr.GetBoolean(2)
                ));

            return list;
        }

        public async Task<DeviceDto?> GetByIdAsync(string id)
        {
            const string sql = @"
SELECT Id, Name, IsEnabled,
       BatteryPercentage, OperatingSystem,
       IpAddress, NetworkName
  FROM Devices
 WHERE Id = @Id";

            using var conn = new SqlConnection(_conn);
            using var cmd  = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            await conn.OpenAsync();
            using var rdr = await cmd.ExecuteReaderAsync();
            if (!await rdr.ReadAsync()) return null;

            return new DeviceDto(
                rdr.GetString("Id"),
                rdr.GetString("Name"),
                rdr.GetBoolean("IsEnabled"),
                rdr.IsDBNull("BatteryPercentage") ? null : rdr.GetInt32("BatteryPercentage"),
                rdr.IsDBNull("OperatingSystem")    ? null : rdr.GetString("OperatingSystem"),
                rdr.IsDBNull("IpAddress")          ? null : rdr.GetString("IpAddress"),
                rdr.IsDBNull("NetworkName")        ? null : rdr.GetString("NetworkName")
            );
        }

        public Task CreateAsync(string csvLine)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(DeviceDto d)
        {
            const string sql = @"
INSERT INTO Devices
  (Id, Name, IsEnabled,
   BatteryPercentage, OperatingSystem,
   IpAddress, NetworkName, DeviceType)
VALUES
  (@Id,@Name,@IsEnabled,
   @Battery,@OS,@IP,@Network,@Type)";

            using var conn = new SqlConnection(_conn);
            using var cmd  = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id",        d.Id);
            cmd.Parameters.AddWithValue("@Name",      d.Name);
            cmd.Parameters.AddWithValue("@IsEnabled", d.IsEnabled);
            cmd.Parameters.AddWithValue("@Battery",   (object?)d.BatteryPercentage ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OS",        (object?)d.OperatingSystem   ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IP",        (object?)d.IpAddress         ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Network",   (object?)d.NetworkName       ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Type",      d.Id.Substring(0,2));
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<bool> UpdateAsync(string id, DeviceDto d)
        {
            const string sql = @"
UPDATE Devices SET
  Name=@Name, IsEnabled=@IsEnabled,
  BatteryPercentage=@Battery,
  OperatingSystem=@OS,
  IpAddress=@IP, NetworkName=@Network
 WHERE Id=@Id";

            using var conn = new SqlConnection(_conn);
            using var cmd  = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id",        id);
            cmd.Parameters.AddWithValue("@Name",      d.Name);
            cmd.Parameters.AddWithValue("@IsEnabled", d.IsEnabled);
            cmd.Parameters.AddWithValue("@Battery",   (object?)d.BatteryPercentage ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OS",        (object?)d.OperatingSystem   ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IP",        (object?)d.IpAddress         ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Network",   (object?)d.NetworkName       ?? DBNull.Value);
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            const string sql = "DELETE FROM Devices WHERE Id=@Id";
            using var conn = new SqlConnection(_conn);
            using var cmd  = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
    }
}
