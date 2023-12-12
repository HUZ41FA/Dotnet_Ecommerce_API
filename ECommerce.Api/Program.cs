using ECommerce.Application.Services;
using ECommerce.Domain.Application;
using ECommerce.Infrastructure.DataAccess.ApplicationDbContext;
using ECommerce.Infrastructure.Email;
using ECommerce.Utilities.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace ECommerce.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            var connectionString = configuration.GetConnectionString("DefaultConnectionString");
            var serverVersion = ServerVersion.AutoDetect(connectionString);
            var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfig>();
            var jwtConfig = configuration.GetSection("JWT").Get<JwtConfig>();
            // Add services to the container.
            builder.Services.AddControllers();

            // For Entityframework
            builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseMySql(configuration.GetConnectionString("DefaultConnectionString"), serverVersion));

            
            builder.Services.AddSingleton(emailConfig);
            builder.Services.AddSingleton(jwtConfig);

            // Add Identity
            builder.Services.AddIdentity<SiteUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDBContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAutoMapper(typeof(Program).Assembly);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });
            builder.Services.AddRouting(options => options.LowercaseUrls = true);
            builder.Services.AddScoped<AuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<EmailService, EmailService>();

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
        }
    }
}
