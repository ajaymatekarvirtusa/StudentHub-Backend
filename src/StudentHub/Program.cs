using Serilog;
using Core.Extensions;
using API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Serilog is the only logging provider; sinks, levels and enrichers come from the "Serilog" section of appsettings.json.
builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services));

// Required by AspNetCoreRateLimit (it stores request counters in IDistributedCache).
builder.Services.AddDistributedMemoryCache();
builder.Services.AddControllers(options =>
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

var rateLimitEnabled = builder.Configuration.GetValue<bool>("RateLimit:Enabled");
if (rateLimitEnabled)
{
    builder.Services.AddDistributedRateLimit(builder.Configuration.GetSection("IpRateLimitingSettings"));
}

builder.Services.AddSwaggerDocumentation();
builder.Services.AddGZipCompression();
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

if (rateLimitEnabled)
{
    app.UseRateLimiting();
}

app.UseResponseCompression();
app.UseExceptionHandleMiddleware();

app.UseSwaggerDocumentation();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
