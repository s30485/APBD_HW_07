using System.Data;
using APBD_HW_07.Business;
using APBD_HW_07.Business.Commands;
using APBD_HW_07.Business.Queries;
using APBD_HW_07.Domain.Models;
using Microsoft.Data.SqlClient;

namespace APBD_HW_07.Data
{
    public class AdoDeviceRepository : IDeviceRepository
    {
        private readonly string _conn;
        private readonly IDeviceCommandFactory _commandFactory;
        private readonly IDeviceQueryHandler<GetAllDevicesQuery, IEnumerable<ShortDeviceDto>> _getAllHandler;
        private readonly IDeviceQueryHandler<GetDeviceByIdQuery, DeviceDto?> _getByIdHandler;
        private readonly IDeviceCommandHandler<CreateDeviceCommand> _createHandler;
        private readonly IDeviceCommandHandler<UpdateDeviceCommand> _updateHandler;

        public AdoDeviceRepository(
            string connectionString,
            IDeviceCommandFactory commandFactory,
            IDeviceQueryHandler<GetAllDevicesQuery, IEnumerable<ShortDeviceDto>> getAllHandler,
            IDeviceQueryHandler<GetDeviceByIdQuery, DeviceDto?> getByIdHandler,
            IDeviceCommandHandler<CreateDeviceCommand> createHandler,
            IDeviceCommandHandler<UpdateDeviceCommand> updateHandler)
        {
            _conn = connectionString;
            _commandFactory = commandFactory;
            _getAllHandler = getAllHandler;
            _getByIdHandler = getByIdHandler;
            _createHandler = createHandler;
            _updateHandler = updateHandler;
        }

        public Task<IEnumerable<ShortDeviceDto>> GetAllAsync()
        {
            return _getAllHandler.HandleAsync(new GetAllDevicesQuery());
        }

        public Task<DeviceDto?> GetByIdAsync(string id)
        {
            return _getByIdHandler.HandleAsync(new GetDeviceByIdQuery(id));
        }

        public Task CreateAsync(DeviceDto d)
        {
            return _createHandler.HandleAsync(new CreateDeviceCommand(d));
        }

        public async Task<bool> UpdateAsync(string id, DeviceDto d)
        {
            try
            {
                await _updateHandler.HandleAsync(new UpdateDeviceCommand(id, d));
                return true;
            }
            catch (InvalidOperationException)
            {
                return false; //optimistic lock failure or not found
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                await _deleteHandler.HandleAsync(new DeleteDeviceCommand(id));
                return true;
            }
            catch (InvalidOperationException)
            {
                return false; // device not found
            }
        }
    }
}
