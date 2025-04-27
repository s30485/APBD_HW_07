namespace APBD_HW_07.Domain.Exceptions;

[Serializable]
public class EmptyBatteryException : Exception 
{
    public EmptyBatteryException() : base("Battery too low to turn on device.") {}
}