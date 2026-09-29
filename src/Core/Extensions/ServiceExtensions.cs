using AspNetCoreRateLimit;
using Core.Handler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions
{

    public static class ServiceExtensions
    {
        public static IServiceCollection AddDistributedRateLimit(this IServiceCollection services, IConfigurationSection section)
        {
            if (section != null)
            {
                // Load in general configuration from appsettings.json
                services.Configure<IpRateLimitOptions>(options => section.Bind(options));
                // Inject Counter and Store Rules
                services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
                services.AddDistributedRateLimiting();
            }
            return services;
        }

        public static IApplicationBuilder UseExceptionHandleMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }

        public static void AddGZipCompression(this IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.Fastest;
            });
        }

        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseIpRateLimiting();
        }
    }
}
