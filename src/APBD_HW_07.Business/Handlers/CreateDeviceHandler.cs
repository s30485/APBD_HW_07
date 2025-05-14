using APBD_HW_07.Business.Commands;
using APBD_HW_07.Business.Interfaces;

namespace APBD_HW_07.Business.Handlers
{
    public class CreateDeviceHandler : IDeviceCommandHandler<CreateDeviceCommand>
    {
        private readonly IDeviceRepository _repo;
        public CreateDeviceHandler(IDeviceRepository repo) => _repo = repo;

        public Task HandleAsync(CreateDeviceCommand command)
            => _repo.CreateAsync(command.Device);
    }
}