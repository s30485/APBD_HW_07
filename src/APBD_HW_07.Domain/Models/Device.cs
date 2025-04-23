namespace APBD_HW_07.Domain.Models;


/// <summary>
/// Abstract class to inherit from.
/// </summary>
public abstract class Device
{
    /// <summary>
    /// Gets or sets the id of device.
    /// </summary>
    public string Id { get; set; }
    
    /// <summary>
    /// Gets or sets name of device.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// gets or sets a value indicating whether the device is current on or off
    /// </summary>
    public bool IsEnabled { get; set; }
    
    /// <summary>
    /// Turns on the device
    /// </summary>
    public abstract void TurnOn(); //abstract because every device type does something a little bit different
    
    /// <summary>
    /// Turns off the device
    /// </summary>
    public void TurnOff() => IsTurnedOn = false;
    
    /// <summary>
    /// toString method to remind the child device to implement this method
    /// </summary>
    /// <returns> Returns a string representation of the device. </returns>
    public override abstract string ToString();
}