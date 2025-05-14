using System;
using System.Collections.Generic;
using System.Linq;
using APBD_HW_07.Business.Interfaces;
using APBD_HW_07.Domain.Models;
using Microsoft.Data.SqlClient;

namespace APBD_HW_07.Business.Factories
{
    public class DeviceCommandFactory : IDeviceCommandFactory
    {
        private readonly IEnumerable<ICreateCommandBuilder> _builders;

        public DeviceCommandFactory(IEnumerable<ICreateCommandBuilder> builders)
        {
            _builders = builders;
        }

        public SqlCommand BuildCreateCommand(SqlConnection conn, DeviceDto device)
        {
            var prefix = device.Id.Substring(0, 2).ToUpperInvariant();
            var builder = _builders.FirstOrDefault(b => b.CanHandle(prefix))
                          ?? throw new ArgumentException($"Error: Unsupported prefix '{prefix}'");
            return builder.Build(conn, device);
        }
    }
}