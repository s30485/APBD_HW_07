using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using APBD_HW_07.Business;
using APBD_HW_07.Domain.Models;
using APBD_HW_07.Domain.Exceptions;

namespace APBD_HW_07.RestAPI.Validators
{
    /// <summary>
    /// Validates a CreateUpdateDeviceDto using DataAnnotations
    /// and also enforces type-specific rules (SOLID’s OCP + SRP).
    /// </summary>
    public class CreateUpdateDeviceDtoValidator : IValidator<CreateUpdateDeviceDto>
    {
        private static readonly HashSet<string> AllowedTypes = new()
            { "SW", "P", "ED" };

        public (bool IsValid, IEnumerable<string> Errors) Validate(CreateUpdateDeviceDto dto)
        {
            var results = new List<ValidationResult>();
            var ctx = new ValidationContext(dto, null, null);

            // 1) DataAnnotations check
            Validator.TryValidateObject(dto, ctx, results, validateAllProperties: true);

            // 2) Type must be one of SW, P, ED
            if (!AllowedTypes.Contains(dto.Type?.ToUpperInvariant() ?? ""))
            {
                results.Add(new ValidationResult(
                    $"Type must be one of: {string.Join(", ", AllowedTypes)}",
                    new[] { nameof(dto.Type) }));
            }

            // 3) Domain‐level sanity check
            //    We can try to construct a dummy domain object to catch its exceptions:
            try
            {
                Device dummy = dto.Type switch
                {
                    "SW" => new Smartwatch { BatteryPercentage = dto.BatteryPercentage ?? 0 },
                    "P"  => new PersonalComputer { OperatingSystem = dto.OperatingSystem ?? "" },
                    "ED" => new EmbeddedDevice 
                            { IpAddress = dto.IpAddress ?? "", 
                              NetworkName = dto.NetworkName ?? "" },
                    _    => null
                };
            }
            catch (ArgumentException ae)
            {
                results.Add(new ValidationResult(ae.Message, new[] { nameof(dto) }));
            }
            catch (EmptyBatteryException ebe)
            {
                // unlikely at creation, but just in case
                results.Add(new ValidationResult(ebe.Message, new[] { nameof(dto.BatteryPercentage) }));
            }
            catch (EmptySystemException ese)
            {
                results.Add(new ValidationResult(ese.Message, new[] { nameof(dto.OperatingSystem) }));
            }
            catch (ConnectionException ce)
            {
                results.Add(new ValidationResult(ce.Message, new[] { nameof(dto.NetworkName) }));
            }

            return (results.Count == 0, results.ConvertAll(r => r.ErrorMessage!));
        }
    }
}
