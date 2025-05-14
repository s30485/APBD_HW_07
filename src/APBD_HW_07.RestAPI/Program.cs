using APBD_HW_07.Business;
using APBD_HW_07.Business.Interfaces;
using APBD_HW_07.Data;
using APBD_HW_07.RestAPI.Validators;

var builder = WebApplication.CreateBuilder(args);

// Data + Business DI
builder.Services.AddSingleton<IDeviceRepository>(sp =>
    new AdoDeviceRepository(
        builder.Configuration.GetConnectionString("DefaultConnection")!
    )
);
//registering dependencies
builder.Services.AddScoped<IDeviceService, DeviceService>(); //when someone asks for IDeviceService then give him DeviceService
//builder.Services.AddScoped<IDeviceFileImporter, DeviceFileImporter>(); //not needed tbh

// Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//the validator class
builder.Services.AddScoped<IValidator<CreateUpdateDeviceDto>, CreateUpdateDeviceDtoValidator>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();