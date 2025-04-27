namespace APBD_HW_07.Domain.Exceptions;

[Serializable]
public class EmptySystemException : Exception
{
    public EmptySystemException() : base("No operating system installed.") {}
}