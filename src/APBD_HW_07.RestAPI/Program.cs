using APBD_HW_07.Business;
using APBD_HW_07.Data;

var builder = WebApplication.CreateBuilder(args);

// Data + Business DI
builder.Services.AddSingleton<IDeviceRepository>(sp =>
    new AdoDeviceRepository(
        builder.Configuration.GetConnectionString("DefaultConnection")!
    )
);
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IDeviceFileImporter, DeviceFileImporter>();

// Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();