using System.Text.Json.Serialization;
using EvacuationPlanningandMonitoringAPI.Helpers;
using EvacuationPlanningandMonitoringAPI.Models.Db;
using EvacuationPlanningandMonitoringAPI.Repositories;
using EvacuationPlanningandMonitoringAPI.Repositories.Interface;
using EvacuationPlanningandMonitoringAPI.Services;
using EvacuationPlanningandMonitoringAPI.Services.Interface;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.WebHost.UseUrls("http://0.0.0.0:8080");
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<EvacuationPlanningandMonitoringDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConnectionString = builder.Configuration.GetSection("Redis:ConnectionString").Value;
    var configuration = ConfigurationOptions.Parse(redisConnectionString);
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddScoped<IDatabase>(sp =>
{
    var connection = sp.GetRequiredService<IConnectionMultiplexer>();
    return connection.GetDatabase();
});

builder.Services.AddScoped<IEvacuationZonesRepositories, EvacuationZoneRepository>();
builder.Services.AddScoped<IVehiclesRepositories, VehiclesRepositories>();
builder.Services.AddScoped<IEvacuationsRepositories, EvacuationsRepositories>();
builder.Services.AddScoped<IRedisRepository, RedisRepository>();

builder.Services.AddScoped<IEvacuationZonesServices, EvacuationZonesServices>();
builder.Services.AddScoped<IVehiclesServices, VehiclesServices>();
builder.Services.AddScoped<IEvacuationsServices, EvacuationsServices>();
builder.Services.AddScoped<IRedisServices, RedisServices>();

builder.Services.AddHostedService<RedisToDbSyncService>();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();
//if (app.Environment.IsDevelopment())
//{
//}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
