namespace APBD_HW_07.Domain
{
    public class Smartwatch : Device
    {
        public int BatteryPercentage { get; set; }
        public override void TurnOn()
        {
            if (BatteryPercentage < 10)
                throw new InvalidOperationException("Battery too low");
            IsEnabled = true;
        }
        
        public override string ToString() =>
            $"Smartwatch [ID:{Id}, Name:{Name}, Battery:{BatteryPercentage}%, On:{IsEnabled}]";
    }
}