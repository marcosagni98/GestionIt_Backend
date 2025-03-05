using API.DependencyInjection;
using API.Extensions;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

builder.ConfigureKestrel();
builder.ConfigureUrls();

// Add services to the container.
builder.Services.AddDependencyInjection(builder.Configuration);

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
app.ConfigureApplication();

app.Run();