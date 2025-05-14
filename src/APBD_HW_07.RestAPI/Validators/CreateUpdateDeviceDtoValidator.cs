using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using APBD_HW_07.Business;
using APBD_HW_07.Domain.Exceptions;
using APBD_HW_07.Domain.Models;

namespace APBD_HW_07.RestAPI.Validators
{
    public class CreateUpdateDeviceDtoValidator : IValidator<CreateUpdateDeviceDto>
    {
        private static readonly HashSet<string> Allowed = new() { "SW","P","ED" };

        public (bool, IEnumerable<string>) Validate(CreateUpdateDeviceDto dto)
        {
            var results = new List<ValidationResult>();
            var ctx = new ValidationContext(dto);
            Validator.TryValidateObject(dto, ctx, results, true);

            if (!Allowed.Contains(dto.Type.ToUpperInvariant()))
                results.Add(new ValidationResult($"Type must be SW, P or ED", new[]{nameof(dto.Type)}));

            try
            {
                Device dummy = dto.Type switch
                {
                    "SW" => new Smartwatch { BatteryPercentage = dto.BatteryPercentage ?? 0 },
                    "P"  => new PersonalComputer { OperatingSystem = dto.OperatingSystem! },
                    "ED" => new EmbeddedDevice { IpAddress = dto.IpAddress!, NetworkName = dto.NetworkName! },
                    _    => null
                };
            }
            catch (Exception ex) when (
                ex is ArgumentException or EmptyBatteryException or EmptySystemException or ConnectionException)
            {
                results.Add(new ValidationResult(ex.Message));
            }

            return (results.Count==0, results.ConvertAll(r=>r.ErrorMessage!));
        }
    }
}