using PaymentGateway.Api.Services;
using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Middleware;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "Payment Gateway API", 
        Version = "v1",
        Description = "A secure payment gateway API built with ASP.NET Core following SOLID principles"
    });
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSwagger", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddSingleton<IPaymentsRepository, PaymentsRepository>();
builder.Services.AddScoped<IPaymentResponseFactory, PaymentResponseFactory>();
builder.Services.AddScoped<IPaymentProcessor, PaymentProcessor>();

builder.Services.AddHttpClient("BankClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:8080");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<IBankService, BankService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Gateway API v1");
        c.DefaultModelsExpandDepth(-1);
        c.ConfigObject.AdditionalItems["servers"] = new[]
        {
            new { url = "https://localhost:7092", description = "HTTPS Server" },
            new { url = "http://localhost:5067", description = "HTTP Server (redirects to HTTPS)" }
        };
    });
    app.UseCors("AllowSwagger");
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }
