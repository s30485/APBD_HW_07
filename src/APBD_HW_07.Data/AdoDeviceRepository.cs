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
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            await using var tran = conn.BeginTransaction();
            try
            {
                //update base Device
                var baseCmd = new SqlCommand(@"
UPDATE Device
   SET Name = @Name,
       IsEnabled = @IsEnabled
 WHERE Id = @Id", conn, tran);
                baseCmd.Parameters.AddWithValue("@Id",        id);
                baseCmd.Parameters.AddWithValue("@Name",      d.Name);
                baseCmd.Parameters.AddWithValue("@IsEnabled", d.IsEnabled);
                var rows = await baseCmd.ExecuteNonQueryAsync();
                if (rows == 0)
                {
                    await tran.RollbackAsync();
                    return false;
                }

                //update subtype
                var prefix = id.Substring(0,2).ToUpperInvariant();
                if (prefix == "SW")
                {
                    var swCmd = new SqlCommand(@"
UPDATE Smartwatch
   SET BatteryPercentage = @Battery
 WHERE DeviceId = @DeviceId", conn, tran);
                    swCmd.Parameters.AddWithValue("@Battery",   d.BatteryPercentage ?? 0);
                    swCmd.Parameters.AddWithValue("@DeviceId",  id);
                    await swCmd.ExecuteNonQueryAsync();
                }
                else if (prefix == "P-")
                {
                    var pcCmd = new SqlCommand(@"
UPDATE PersonalComputer
   SET OperationSystem = @OS
 WHERE DeviceId = @DeviceId", conn, tran);
                    pcCmd.Parameters.AddWithValue("@OS",        d.OperatingSystem ?? string.Empty);
                    pcCmd.Parameters.AddWithValue("@DeviceId",  id);
                    await pcCmd.ExecuteNonQueryAsync();
                }
                else if (prefix == "ED")
                {
                    var edCmd = new SqlCommand(@"
UPDATE Embedded
   SET IpAddress   = @IP,
       NetworkName = @Network
 WHERE DeviceId = @DeviceId", conn, tran);
                    edCmd.Parameters.AddWithValue("@IP",        d.IpAddress    ?? string.Empty);
                    edCmd.Parameters.AddWithValue("@Network",   d.NetworkName  ?? string.Empty);
                    edCmd.Parameters.AddWithValue("@DeviceId",  id);
                    await edCmd.ExecuteNonQueryAsync();
                }
                else
                {
                    throw new ArgumentException($"Unknown type prefix '{prefix}'");
                }

                await tran.CommitAsync();
                return true;
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            await using var tran = conn.BeginTransaction();
            try
            {
                //remove subtype row first, so the constraint error is fixed
                var prefix = id.Substring(0,2).ToUpperInvariant();
                if (prefix == "SW")
                    await new SqlCommand("DELETE FROM Smartwatch WHERE DeviceId = @DeviceId", conn, tran)
                            { Parameters = { new SqlParameter("@DeviceId", id) } }
                        .ExecuteNonQueryAsync();
                else if (prefix == "P-")
                    await new SqlCommand("DELETE FROM PersonalComputer WHERE DeviceId = @DeviceId", conn, tran)
                            { Parameters = { new SqlParameter("@DeviceId", id) } }
                        .ExecuteNonQueryAsync();
                else if (prefix == "ED")
                    await new SqlCommand("DELETE FROM Embedded WHERE DeviceId = @DeviceId", conn, tran)
                            { Parameters = { new SqlParameter("@DeviceId", id) } }
                        .ExecuteNonQueryAsync();
                else
                    throw new ArgumentException($"Unknown type prefix '{prefix}'");

                //delete base Device, should be no sub device now 
                var baseCmd = new SqlCommand("DELETE FROM Device WHERE Id = @Id", conn, tran);
                baseCmd.Parameters.AddWithValue("@Id", id);
                var rows = await baseCmd.ExecuteNonQueryAsync();

                await tran.CommitAsync();
                return rows > 0;
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }
    }
}
