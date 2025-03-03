using API.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDependencyInjection(builder.Configuration);

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
app.ConfigureApplication(app.Environment);

app.Run();