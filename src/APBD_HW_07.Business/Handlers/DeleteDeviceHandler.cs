namespace DefaultNamespace;

using System;
using System.Data;
using System.Threading.Tasks;
using APBD_HW_07.Business.Commands;
using APBD_HW_07.Domain.Models;
using Microsoft.Data.SqlClient;

namespace APBD_HW_07.Business.Handlers
{
    public class DeleteDeviceHandler : IDeviceCommandHandler<DeleteDeviceCommand>
    {
        private readonly string _connectionString;

        public DeleteDeviceHandler(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task HandleAsync(DeleteDeviceCommand command)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var tran = conn.BeginTransaction();

            try
            {
                var prefix = command.Id.Substring(0, 2).ToUpperInvariant();
                if (prefix == "SW")
                {
                    await new SqlCommand("DELETE FROM Smartwatch WHERE DeviceId = @DeviceId", conn, tran)
                    {
                        Parameters = { new SqlParameter("@DeviceId", command.Id) }
                    }.ExecuteNonQueryAsync();
                }
                else if (prefix == "P-")
                {
                    await new SqlCommand("DELETE FROM PersonalComputer WHERE DeviceId = @DeviceId", conn, tran)
                    {
                        Parameters = { new SqlParameter("@DeviceId", command.Id) }
                    }.ExecuteNonQueryAsync();
                }
                else if (prefix == "ED")
                {
                    await new SqlCommand("DELETE FROM Embedded WHERE DeviceId = @DeviceId", conn, tran)
                    {
                        Parameters = { new SqlParameter("@DeviceId", command.Id) }
                    }.ExecuteNonQueryAsync();
                }
                else
                {
                    throw new ArgumentException($"Unknown type prefix '{prefix}'");
                }

                var baseCmd = new SqlCommand("DELETE FROM Device WHERE Id = @Id", conn, tran);
                baseCmd.Parameters.AddWithValue("@Id", command.Id);
                var rows = await baseCmd.ExecuteNonQueryAsync();

                if (rows == 0)
                    throw new InvalidOperationException("Delete failed: device not found");

                await tran.CommitAsync();
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }
    }
}
