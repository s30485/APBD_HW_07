using System.Text.RegularExpressions;
using APBD_HW_07.Domain.Exceptions;
using APBD_HW_07.Domain.Models;

namespace APBD_HW_07.Domain.Models
{
    /// <summary>
    /// Represents an embedded device with ip adress and network name
    /// </summary>
    public class EmbeddedDevice : Device
    {
    
        private static readonly Regex IpRegex = new(@"^(\d{1,3}\.){3}\d{1,3}$");
        private string _ipAddress;
    
        /// <summary>
        /// Gets or sets ip address of device.
        /// </summary>
        /// Throws <exception cref="ArgumentException"></exception>
        public string IpAddress 
        { 
            get => _ipAddress;
            set
            {
                if (!IpRegex.IsMatch(value)) throw new ArgumentException("Invalid IP address format.");
                //matches the format with regexes, if the format is not findable then throws a ArgumentException
                _ipAddress = value;
            }
        }
    
        /// <summary>
        /// Gets or sets network name of the device
        /// </summary>
        public string NetworkName { get; set; }
    
        /// <summary>
        /// Attempts to commect the device to its network
        /// </summary>
        /// Throws <exception cref="ConnectionException"></exception>
        public void Connect()
        {
            if (!NetworkName.Contains("MD Ltd.")) throw new ConnectionException();
            //many built-in functions like python, I like c# already
            //if NetworkName does not contain MD Ltd then throw exeption
            Console.WriteLine("Connected successfully.");
        }

        /// <summary>
        /// overrided turn on method.
        /// </summary>
        public override void TurnOn()
        {
            Connect();
            //used in turn on like task specifies
            IsEnabled = true;
        }

        /// <summary>
        /// overrided toString method
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"Embedded Device [ID: {Id}, Name: {Name}, IP: {IpAddress}, Network: {NetworkName}, On: {IsEnabled}]";
    }
}