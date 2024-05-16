using AudioGeraImagemAPI.API.Configurations;
using System.Diagnostics.CodeAnalysis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSerilogConfiguration(builder.Configuration);
builder.Host.UseSerilogConfiguration();
builder.Services.AddHttpClient();
builder.Services.AddMediatRConfiguration();
builder.Services.AddRetryPolicy();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDepencyInjection();
builder.Services.AddBusConfiguration(builder.Configuration);
builder.Services.AddDbContextConfiguration(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

[ExcludeFromCodeCoverage]
public static partial class Program { }
