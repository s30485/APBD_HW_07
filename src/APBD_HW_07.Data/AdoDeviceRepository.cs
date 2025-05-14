using System.Data;
using APBD_HW_07.Business;
using Microsoft.Data.SqlClient;
using APBD_HW_07.Domain.Models;

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
            using var cmd = new SqlCommand(sql, conn);
            await conn.OpenAsync();
            using var rdr = await cmd.ExecuteReaderAsync();

            while (await rdr.ReadAsync())
            {
                list.Add(new ShortDeviceDto(
                    rdr.GetString(0),
                    rdr.GetString(1),
                    rdr.GetInt32(2)
                ));
            }

            return list;
        }

        public async Task<DeviceDto?> GetByIdAsync(string id)
        {
            const string sql = @"SELECT d.Id, d.Name, d.IsEnabled, sw.BatteryPercentage, pc.OperationSystem, ed.IpAddress, ed.NetworkName, d.RowVersion FROM Device AS d
                                    LEFT JOIN Smartwatch AS sw ON sw.DeviceId = d.Id
                                    LEFT JOIN PersonalComputer AS pc ON pc.DeviceId = d.Id
                                    LEFT JOIN Embedded AS ed ON ed.DeviceId = d.Id
                                    WHERE d.Id = @Id";

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            using var rdr = await cmd.ExecuteReaderAsync();
            if (!await rdr.ReadAsync()) return null;

            return new DeviceDto(
                rdr.GetString(0),
                rdr.GetString(1),
                rdr.GetInt32(2),
                rdr.IsDBNull(3) ? null : rdr.GetInt32(3),
                rdr.IsDBNull(4) ? null : rdr.GetString(4),
                rdr.IsDBNull(5) ? null : rdr.GetString(5),
                rdr.IsDBNull(6) ? null : rdr.GetString(6),
                rdr.GetFieldValue<byte[]>(7) // rowVersion
            );
        }

        public async Task CreateAsync(DeviceDto d)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("CreateDevice", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", d.Id);
            cmd.Parameters.AddWithValue("@Name", d.Name);
            cmd.Parameters.AddWithValue("@IsEnabled", d.IsEnabled);
            cmd.Parameters.AddWithValue("@Type", d.Id.Substring(0, 2).ToUpperInvariant());
            cmd.Parameters.AddWithValue("@Battery", (object?)d.BatteryPercentage ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OS", (object?)d.OperatingSystem ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IP", (object?)d.IpAddress ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Network", (object?)d.NetworkName ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<bool> UpdateAsync(string id, DeviceDto d)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            await using var tran = conn.BeginTransaction();
            try
            {
                var baseCmd = new SqlCommand(@"UPDATE Device SET Name = @Name, IsEnabled = @IsEnabled WHERE Id = @Id AND RowVersion = @RowVersion; SELECT @@ROWCOUNT;", conn, tran);

                baseCmd.Parameters.AddWithValue("@Id", id);
                baseCmd.Parameters.AddWithValue("@Name", d.Name);
                baseCmd.Parameters.AddWithValue("@IsEnabled", d.IsEnabled);
                baseCmd.Parameters.AddWithValue("@RowVersion", d.RowVersion);

                var rows = Convert.ToInt32(await baseCmd.ExecuteScalarAsync());
                if (rows == 0)
                {
                    await tran.RollbackAsync();
                    return false; //conflict
                }

                var prefix = id.Substring(0, 2).ToUpperInvariant();
                if (prefix == "SW")
                {
                    var swCmd = new SqlCommand(@"UPDATE Smartwatch SET BatteryPercentage = @Battery WHERE DeviceId = @DeviceId", conn, tran);
                    swCmd.Parameters.AddWithValue("@Battery", d.BatteryPercentage ?? 0);
                    swCmd.Parameters.AddWithValue("@DeviceId", id);
                    await swCmd.ExecuteNonQueryAsync();
                }
                else if (prefix == "P-")
                {
                    var pcCmd = new SqlCommand(@"UPDATE PersonalComputer SET OperationSystem = @OS WHERE DeviceId = @DeviceId", conn, tran);
                    pcCmd.Parameters.AddWithValue("@OS", d.OperatingSystem ?? string.Empty);
                    pcCmd.Parameters.AddWithValue("@DeviceId", id);
                    await pcCmd.ExecuteNonQueryAsync();
                }
                else if (prefix == "ED")
                {
                    var edCmd = new SqlCommand(@"UPDATE Embedded SET IpAddress = @IP, NetworkName = @Network WHERE DeviceId = @DeviceId", conn, tran);
                    edCmd.Parameters.AddWithValue("@IP", d.IpAddress ?? string.Empty);
                    edCmd.Parameters.AddWithValue("@Network", d.NetworkName ?? string.Empty);
                    edCmd.Parameters.AddWithValue("@DeviceId", id);
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
                var prefix = id.Substring(0, 2).ToUpperInvariant();
                if (prefix == "SW")
                    await new SqlCommand("DELETE FROM Smartwatch WHERE DeviceId = @DeviceId", conn, tran)
                    { Parameters = { new SqlParameter("@DeviceId", id) } }.ExecuteNonQueryAsync();
                else if (prefix == "P-")
                    await new SqlCommand("DELETE FROM PersonalComputer WHERE DeviceId = @DeviceId", conn, tran)
                    { Parameters = { new SqlParameter("@DeviceId", id) } }.ExecuteNonQueryAsync();
                else if (prefix == "ED")
                    await new SqlCommand("DELETE FROM Embedded WHERE DeviceId = @DeviceId", conn, tran)
                    { Parameters = { new SqlParameter("@DeviceId", id) } }.ExecuteNonQueryAsync();
                else
                    throw new ArgumentException($"Unknown type prefix '{prefix}'");

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
