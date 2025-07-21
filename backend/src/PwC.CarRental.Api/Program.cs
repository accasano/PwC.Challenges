using PwC.CarRental.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomSwagger();
builder.Services.AddCustomAuth(builder.Configuration);
builder.Services.AddCustomConfigurations(builder.Configuration);
builder.Services.AddCustomServices(builder.Configuration);
builder.Services.AddCustomCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCustomSwagger();
}

app.UseHttpsRedirection();
app.UseCustomAuth();
app.UseCustomCors();
app.UseCustomMiddlewares();
app.MapApiEndpoints();
app.MigrateDatabase();

app.Run();
