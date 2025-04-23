namespace APBD_HW_07.Domain.Models;

/// <summary>
/// Represents a personal computer device.
/// </summary>
public class PersonalComputer : Device
{
    /// <summary>
    /// Gets or sets the operating system installed on the PC.
    /// </summary>
    public string OperatingSystem { get; set; }
    
    /// <summary>
    /// overrided turn on the devide
    /// </summary>
    /// <exception cref="EmptySystemException"></exception>
    public override void TurnOn()
    {
        if (string.IsNullOrEmpty(OperatingSystem)) throw new EmptySystemException();
        //if no operating system -> throw exception
        IsEnabled = true;
    }

    /// <summary>
    /// overrided toString
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"PC [ID: {Id}, Name: {Name}, OS: {OperatingSystem ?? "None"}, On: {IsTurnedOn}]";
}