using Microsoft.OpenApi;

namespace API.Extensions
{
    public static class SwaggerExtensions
    {
        private const string BearerScheme = "Bearer";

        /// <summary>Registers Swagger/OpenAPI generation with JWT bearer support.</summary>
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(BearerScheme, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Paste the accessToken from POST /api/Auth/token (without the word 'Bearer')."
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(BearerScheme, document)] = []
                });
            });

            return services;
        }

        /// <summary>Enables the Swagger JSON endpoint and Swagger UI (Development only).</summary>
        public static WebApplication UseSwaggerDocumentation(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            return app;
        }
    }
}
