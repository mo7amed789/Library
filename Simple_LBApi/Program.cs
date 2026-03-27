using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using Simple_LBApi.Data;
using Simple_LBApi.Domain.Enities;
using Simple_LBApi.Domain.Settings;
using Simple_LBApi.Extensions;
using Simple_LBApi.Seeders;
using Simple_LBApi.Services.Implementation;
using Simple_LBApi.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace Simple_LBApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // ================= SERVICES =================
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            // DB
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));
            // JWT Settings
            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("JwtSettings"));

            // Authentication
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    var jwt = builder.Configuration
                    .GetSection("JwtSettings")
                    .Get<JwtSettings>()
                    ?? throw new Exception("JWT settings are missing");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,

                        ValidIssuer = jwt.Issuer,
                        ValidAudience = jwt.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwt.Key))
                    };
                });

            builder.Services.AddAuthorization();
            // DI
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddAppServices(builder.Configuration);
            builder.Services.AddScoped<ILoanService, LoanService>();
            builder.Services.Configure<LibrarySettings>(
            builder.Configuration.GetSection("LibrarySettings"));

            // Swagger (NSwag)
            builder.Services.AddOpenApiDocument(config =>
            {
                config.Title = "LMS API";
                config.Version = "v1";

                config.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Bearer {token}"
                });

                config.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("JWT")
                );
            });

            var app = builder.Build();
            // ================= SEED DATA =================
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<AdminSeeder>();
                await seeder.SeedAsync();
            }
            var json = await File.ReadAllTextAsync("SeedData/books.json");
            var books = JsonSerializer.Deserialize<List<Book>>(json);
            // ================= MIDDLEWARE =================
            if (app.Environment.IsDevelopment())
            {
                app.UseOpenApi();
                app.UseSwaggerUi();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}