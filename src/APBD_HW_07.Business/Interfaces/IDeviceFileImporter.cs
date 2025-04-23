namespace APBD_HW_07.Business
{
    /// <summary>
    /// Bulk-load devices from a plain text file (one device-per-line).
    /// Invalid lines are skipped.
    /// </summary>
    public interface IDeviceFileImporter
    {
        Task ImportFromFileAsync(string filePath);
    }
}