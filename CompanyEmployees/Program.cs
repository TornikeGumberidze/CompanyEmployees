using CompanyEmployees.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using NLog;
var builder = WebApplication.CreateBuilder(args);
LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureSqlContext(builder.Configuration);
// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseForwardedHeaders(
    new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All }
    );
app.UseCors("CorsPolicy");


app.UseAuthorization();

app.MapControllers();

app.Run();
