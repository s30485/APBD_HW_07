using System;
using APBD_HW_07.Business.Commands;
using APBD_HW_07.Business.Interfaces;

namespace APBD_HW_07.Business.Handlers
{
    public class UpdateDeviceHandler : IDeviceCommandHandler<UpdateDeviceCommand>
    {
        private readonly IDeviceRepository _repo;
        public UpdateDeviceHandler(IDeviceRepository repo) => _repo = repo;

        public async Task HandleAsync(UpdateDeviceCommand command)
        {
            var ok = await _repo.UpdateAsync(command.Id, command.Device);
            if (!ok) throw new InvalidOperationException("Invalid version conflict");
        }
    }
}