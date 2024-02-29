using ECommerce.Application.Services;
using ECommerce.Domain.Abstractions.IServices.Application;
using ECommerce.Domain.Abstractions.IUnitOfWork;
using ECommerce.Domain.Models.Application;
using ECommerce.Infrastructure.DataAccess.ApplicationDbContext;
using ECommerce.Infrastructure.DataAccess.UnitOfWork;
using ECommerce.Infrastructure.Email;
using ECommerce.Utilities.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

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

            builder.Services.AddControllers();
            
            // Adding DbContext
            builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseMySql(configuration.GetConnectionString("DefaultConnectionString"), serverVersion));
            
            // Adding identity
            builder.Services.AddIdentity<SiteUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDBContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = false, // only for development
                        ValidateIssuer = false, // only for development
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"])),
                        RequireExpirationTime = false, // Will update when refresh token is added
                        ValidateLifetime = true
                    };
                });

            // Auto Mappers
            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            // My Services
            builder.Services.AddSingleton(emailConfig);
            builder.Services.AddSingleton(jwtConfig);
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IApplicationAuthenticationService, ApplicationAuthenticationService>();
            builder.Services.AddScoped<ISiteUserService, SiteUserService>();

            // Configure the routing to use lowercase
            builder.Services.AddRouting(options => options.LowercaseUrls = true);

            builder.Services.AddEndpointsApiExplorer();

            // Adding swagger with authentication token feature
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
