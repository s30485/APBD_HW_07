using APBD_HW_07.Domain;
using APBD_HW_07.Domain.Models;

namespace APBD_HW_07.Business

{
    public class DeviceFactory : IDeviceFactory
    {
        public Device? CreateFromCsv(string line)
        {
            var p = line.Split(',');
            if (p.Length < 2) return null;
            var id   = p[0].Trim();
            var name = p[1].Trim();
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name))
                return null;

            return id[..2] switch
            {
                "SW" => new Smartwatch
                {
                    Id = id,
                    Name = name,
                    IsEnabled         = bool.Parse(p[2]),
                    BatteryPercentage = int.Parse(p[3].TrimEnd('%'))
                },
                "P-" => new PersonalComputer
                {
                    Id              = id,
                    Name            = name,
                    IsEnabled       = bool.Parse(p[2]),
                    OperatingSystem = p.ElementAtOrDefault(3) ?? ""
                },
                "ED" => new EmbeddedDevice
                {
                    Id          = id,
                    Name        = name,
                    IsEnabled   = bool.Parse(p[2]),
                    IpAddress   = p[3],
                    NetworkName = p[4]
                },
                _ => null
            };
        }
    }
}