using Data.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using API.Services;
using API.Validators;
using Microsoft.AspNetCore.Identity;
using Repositories.DBContext;
using Repositories.Interface;
using Repositories;
using Services.Interface;
using Services.Interface.Masters;
using Services.Service;
using Services.Service.Masters;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConfigSettings>(configuration.GetSection("ConnectionStrings"));

            services.AddScoped<StudentDbContext>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ITokenService, TokenService>();

            // Masters: ONE generic repository for every master table (Country, State, ...).
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ICountryService, CountryService>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    // 120 = SQL Server 2014, so EF never generates SQL that 2014 does not support.
                    sql => sql.UseCompatibilityLevel(120)));

            // Add Validation
            services.AddFluentValidationAutoValidation()
                .AddValidatorsFromAssemblyContaining<StudentDetailValidator>();

            // Register Identity services
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // JWT bearer authentication (must come AFTER AddIdentity so it overrides Identity's cookie defaults)
            services.AddJwtAuthentication(configuration);

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("Jwt");
            services.Configure<JwtSettings>(jwtSection);

            var jwt = jwtSection.Get<JwtSettings>()
                      ?? throw new InvalidOperationException("The 'Jwt' section is missing in appsettings.json.");

            if (string.IsNullOrWhiteSpace(jwt.SecretKey) || Encoding.UTF8.GetByteCount(jwt.SecretKey) < 32)
                throw new InvalidOperationException("Jwt:SecretKey must be at least 32 bytes long for HS256.");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = "unique_name",
                    RoleClaimType = "role"
                };
            });

            services.AddAuthorization();

            return services;
        }
    }
}
