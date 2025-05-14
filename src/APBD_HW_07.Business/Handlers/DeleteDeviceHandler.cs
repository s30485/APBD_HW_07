using System;
using APBD_HW_07.Business.Commands;
using APBD_HW_07.Business.Interfaces;

namespace APBD_HW_07.Business.Handlers
{
    public class DeleteDeviceHandler : IDeviceCommandHandler<DeleteDeviceCommand>
    {
        private readonly IDeviceRepository _repo;
        public DeleteDeviceHandler(IDeviceRepository repo) => _repo = repo;

        public async Task HandleAsync(DeleteDeviceCommand command)
        {
            var ok = await _repo.DeleteAsync(command.Id);
            if (!ok) throw new InvalidOperationException("Not found");
        }
    }
}