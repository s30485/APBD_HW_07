namespace APBD_HW_07.RestAPI.Validators
{
    public interface IValidator<T>
    {
        /// <summary>
        /// Validates <paramref name="instance"/>, returning whether it's valid 
        /// and a list of error messages otherwise.
        /// </summary>
        (bool IsValid, IEnumerable<string> Errors) Validate(T instance);
    }
}
