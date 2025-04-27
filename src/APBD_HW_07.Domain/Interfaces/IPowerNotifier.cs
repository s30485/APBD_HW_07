namespace APBD_HW_07.Domain.Interfaces;

/// <summary>
/// Defines a contract for notifying about low battery.
/// </summary>
public interface IPowerNotifier
{
    /// <summary>
    /// Notify about low battery.
    /// </summary>
    void NotifyLowBattery();
}