using System.ComponentModel.DataAnnotations;

namespace APBD_HW_07.Domain
{
    public class PersonalComputer : Device
    {
        [MaxLength(50)]
        public string OperatingSystem { get; set; }
        public override void TurnOn()
        {
            if (string.IsNullOrWhiteSpace(OperatingSystem))
                throw new InvalidOperationException("No OS installed");
            IsEnabled = true;
        }
        
        public override string ToString() =>
            $"PC [ID:{Id}, Name:{Name}, OS:{OperatingSystem}, On:{IsEnabled}]";
    }
}