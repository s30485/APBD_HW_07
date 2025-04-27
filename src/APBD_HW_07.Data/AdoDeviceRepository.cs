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
            const string sql = @"SELECT Id, Name, IsEnabled FROM Device";

            using var conn = new SqlConnection(_conn);
            using var cmd  = new SqlCommand(sql, conn);
            await conn.OpenAsync();
            using var rdr = await cmd.ExecuteReaderAsync();

            while (await rdr.ReadAsync())
            {
                list.Add(new ShortDeviceDto(
                    rdr.GetString(0),      // Id
                    rdr.GetString(1),      // Name
                    rdr.GetInt32(2)
                ));
            }

            return list;
        }

        public async Task<DeviceDto?> GetByIdAsync(string id)
        {
            const string sql = @"
SELECT
  d.Id,
  d.Name,
  d.IsEnabled,
  sw.BatteryPercentage,
  pc.OperationSystem,
  ed.IpAddress,
  ed.NetworkName
FROM Device AS d
LEFT JOIN Smartwatch        AS sw ON sw.DeviceId = d.Id
LEFT JOIN PersonalComputer AS pc ON pc.DeviceId = d.Id
LEFT JOIN Embedded         AS ed ON ed.DeviceId = d.Id
WHERE d.Id = @Id";

            using var conn = new SqlConnection(_conn);
            using var cmd  = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            using var rdr = await cmd.ExecuteReaderAsync();
            if (!await rdr.ReadAsync()) return null;

            return new DeviceDto(
                rdr.GetString(0),                  // Id
                rdr.GetString(1),                  // Name
                rdr.GetInt32(2),                 // IsEnabled
                rdr.IsDBNull(3) ? null : rdr.GetInt32(3),     // BatteryPercentage
                rdr.IsDBNull(4) ? null : rdr.GetString(4),    // OperatingSystem
                rdr.IsDBNull(5) ? null : rdr.GetString(5),    // IpAddress
                rdr.IsDBNull(6) ? null : rdr.GetString(6)     // NetworkName
            );
        }

        
        //creating a device
        
        public async Task CreateAsync(DeviceDto d)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            //Start a transaction
            await using var tran = conn.BeginTransaction();
            try
            {
                //insert into base Device table
                await using (var cmd = new SqlCommand(@"
INSERT INTO Device (Id, Name, IsEnabled)
VALUES (@Id, @Name, @IsEnabled)", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@Id",        d.Id);
                    cmd.Parameters.AddWithValue("@Name",      d.Name);
                    cmd.Parameters.AddWithValue("@IsEnabled", d.IsEnabled);
                    await cmd.ExecuteNonQueryAsync();
                }

                //insert into the appropriate subtype table
                var prefix = d.Id.Substring(0, 2).ToUpperInvariant();
                if (prefix == "SW")
                {
                    //generate a new integer ID for Smartwatch
                    await using (var cmdMax = new SqlCommand(
                        "SELECT ISNULL(MAX(Id), 0) + 1 FROM Smartwatch", conn, tran))
                    {
                        var newId = Convert.ToInt32(await cmdMax.ExecuteScalarAsync());
                        await using (var cmdSw = new SqlCommand(@"
INSERT INTO Smartwatch (Id, BatteryPercentage, DeviceId)
VALUES (@Id, @Battery, @DeviceId)", conn, tran))
                        {
                            cmdSw.Parameters.AddWithValue("@Id",          newId);
                            cmdSw.Parameters.AddWithValue("@Battery",     d.BatteryPercentage ?? 0);
                            cmdSw.Parameters.AddWithValue("@DeviceId",    d.Id);
                            await cmdSw.ExecuteNonQueryAsync();
                        }
                    }
                }
                else if (prefix == "P-")
                {
                    await using (var cmdMax = new SqlCommand(
                        "SELECT ISNULL(MAX(Id), 0) + 1 FROM PersonalComputer", conn, tran))
                    {
                        var newId = Convert.ToInt32(await cmdMax.ExecuteScalarAsync());
                        await using (var cmdPc = new SqlCommand(@"
INSERT INTO PersonalComputer (Id, OperationSystem, DeviceId)
VALUES (@Id, @OS, @DeviceId)", conn, tran))
                        {
                            cmdPc.Parameters.AddWithValue("@Id",       newId);
                            cmdPc.Parameters.AddWithValue("@OS",       d.OperatingSystem ?? string.Empty);
                            cmdPc.Parameters.AddWithValue("@DeviceId", d.Id);
                            await cmdPc.ExecuteNonQueryAsync();
                        }
                    }
                }
                else if (prefix == "ED")
                {
                    await using (var cmdMax = new SqlCommand(
                        "SELECT ISNULL(MAX(Id), 0) + 1 FROM Embedded", conn, tran))
                    {
                        var newId = Convert.ToInt32(await cmdMax.ExecuteScalarAsync());
                        await using (var cmdEd = new SqlCommand(@"
INSERT INTO Embedded (Id, IpAddress, NetworkName, DeviceId)
VALUES (@Id, @IP, @Network, @DeviceId)", conn, tran))
                        {
                            cmdEd.Parameters.AddWithValue("@Id",          newId);
                            cmdEd.Parameters.AddWithValue("@IP",          d.IpAddress     ?? string.Empty);
                            cmdEd.Parameters.AddWithValue("@Network",     d.NetworkName   ?? string.Empty);
                            cmdEd.Parameters.AddWithValue("@DeviceId",    d.Id);
                            await cmdEd.ExecuteNonQueryAsync();
                        }
                    }
                }
                else
                {
                    throw new ArgumentException($"Unknown device type prefix '{prefix}'.");
                }

                //commit both inserts
                await tran.CommitAsync();
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(string id, DeviceDto d)
        {
            const string sql = @"UPDATE Device SET 
                  Name=@Name, 
                  IsEnabled=@IsEnabled,
                  BatteryPercentage=@Battery,
                  OperatingSystem=@OS,
                  IpAddress=@IP, 
                  NetworkName=@Network
                  WHERE Id=@Id";

            using var conn = new SqlConnection(_conn);
            using var cmd  = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@Id",        id);
            cmd.Parameters.AddWithValue("@Name",      d.Name);
            cmd.Parameters.AddWithValue("@IsEnabled", d.IsEnabled);                   // bool
            cmd.Parameters.AddWithValue("@Battery",   (object?)d.BatteryPercentage ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OS",        (object?)d.OperatingSystem   ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IP",        (object?)d.IpAddress         ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Network",   (object?)d.NetworkName       ?? DBNull.Value);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            const string sql = "DELETE FROM Device WHERE Id=@Id";

            using var conn = new SqlConnection(_conn);
            using var cmd  = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
    }
}
