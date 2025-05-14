using APBD_HW_07.Business.Interfaces;

namespace APBD_HW_07.Business
{
    public class DeviceFileImporter : IDeviceFileImporter
    {
        private readonly IDeviceService _service;
        public DeviceFileImporter(IDeviceService service) 
            => _service = service;

        public async Task ImportFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            foreach (var line in File.ReadLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                try
                {
                    await _service.CreateFromFileLineAsync(line);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Skipping invalid line: “{line}” → {ex.Message}");
                }
            }
        }
    }
}