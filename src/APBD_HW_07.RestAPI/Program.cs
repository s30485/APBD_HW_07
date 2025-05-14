using APBD_HW_07.Business;
using APBD_HW_07.Business.Builders;
using APBD_HW_07.Business.Commands;
using APBD_HW_07.Business.Factories;
using APBD_HW_07.Business.Handlers;
using APBD_HW_07.Business.Interfaces;
using APBD_HW_07.Business.Queries;
using APBD_HW_07.Data;
using APBD_HW_07.RestAPI.Validators;

var builder = WebApplication.CreateBuilder(args);

//builders and factory
builder.Services.AddScoped<ICreateCommandBuilder, SmartwatchCreateCommandBuilder>();
builder.Services.AddScoped<ICreateCommandBuilder, PersonalComputerCreateCommandBuilder>();
builder.Services.AddScoped<ICreateCommandBuilder, EmbeddedCreateCommandBuilder>();
builder.Services.AddScoped<IDeviceCommandFactory, DeviceCommandFactory>();

//repo
builder.Services.AddScoped<IDeviceRepository>(sp =>
    new AdoDeviceRepository(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        sp.GetRequiredService<IDeviceCommandFactory>()
    )
);

//handlers
builder.Services.AddScoped<IDeviceQueryHandler<GetAllDevicesQuery, IEnumerable<ShortDeviceDto>>, GetAllDevicesHandler>();
builder.Services.AddScoped<IDeviceQueryHandler<GetDeviceByIdQuery, DeviceDto?>, GetDeviceByIdHandler>();
builder.Services.AddScoped<IDeviceCommandHandler<CreateDeviceCommand>, CreateDeviceHandler>();
builder.Services.AddScoped<IDeviceCommandHandler<UpdateDeviceCommand>, UpdateDeviceHandler>();
builder.Services.AddScoped<IDeviceCommandHandler<DeleteDeviceCommand>, DeleteDeviceHandler>();

//validators
builder.Services.AddScoped<IValidator<CreateUpdateDeviceDto>, CreateUpdateDeviceDtoValidator>();

//API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();