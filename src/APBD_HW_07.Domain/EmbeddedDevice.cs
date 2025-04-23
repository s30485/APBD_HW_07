using System.Text.RegularExpressions;

namespace APBD_HW_07.Domain
{
    public class EmbeddedDevice : Device
    {
        private static readonly Regex IpRegex = new(@"^(\d{1,3}\.){3}\d{1,3}$");
        private string _ipAddress;

        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                if (!IpRegex.IsMatch(value))
                    throw new ArgumentException("Invalid IP address format.");
                _ipAddress = value;
            }
        }

        public string NetworkName { get; set; }

        public override void TurnOn()
        {
            // imagine network connect logic here...
            IsEnabled = true;
        }

        public override string ToString() =>
            $"ED [ID:{Id}, Name:{Name}, IP:{IpAddress}, Net:{NetworkName}, On:{IsEnabled}]";
    }
}