namespace APBD_HW_07.Domain.Models;

/// <summary>
/// Represents a smartwatch device with battery monitoring.
/// </summary>
public class Smartwatch : Device, IPowerNotifier
{
    private int _batteryPercentage; //private fields start from _
    
    /// <summary>
    /// Gets or sets the battery percentage.
    /// Triggers a low battery warning if value drops below 20%.
    /// </summary>
    /// if % outsice od range throws <exception cref="ArgumentOutOfRangeException"></exception>
    public int BatteryPercentage 
    { 
        get => _batteryPercentage;
        set
        {
            if (value < 0 || value > 100) throw new ArgumentOutOfRangeException("Battery percentage must be between 0 and 100.");
            _batteryPercentage = value;
            if (_batteryPercentage < 20) NotifyLowBattery();
        }
    }

    /// <summary>
    /// Sends a warning when the battery is low.
    /// </summary>
    public void NotifyLowBattery() => Console.WriteLine("Warning: Battery is low!");

    /// <summary>
    /// overrided turn on the devide
    /// </summary>
    /// <exception cref="EmptyBatteryException"></exception>
    public override void TurnOn()
    {
        if (BatteryPercentage < 11) throw new EmptyBatteryException();
        IsEnabled = true;
        BatteryPercentage -= 10;
    }
    
    /// <summary>
    /// overrided toString
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Smartwatch [ID: {Id}, Name: {Name}, Battery: {BatteryPercentage}%, On: {IsTurnedOn}]";
}