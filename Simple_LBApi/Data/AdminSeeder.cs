using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Simple_LBApi.Data;
using Simple_LBApi.Domain.Enities;
using Simple_LBApi.Domain.Enums;
using Simple_LBApi.Services.Implementation;

namespace Simple_LBApi.Seeders
{
    public class AdminSeeder
    {
        private readonly AppDbContext _db;
        private readonly AdminSettings _settings;
        private readonly ILogger<AdminSeeder> _logger;

        public AdminSeeder(
            AppDbContext db,
            IOptions<AdminSettings> settings,
            ILogger<AdminSeeder> logger)
        {
            _db = db;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.Email))
                    throw new Exception("Admin email is not configured");

                if (string.IsNullOrWhiteSpace(_settings.Password) || _settings.Password.Length < 8)
                    throw new Exception("Admin password is weak or not configured");

                var exists = await _db.Users
                    .AnyAsync(u => u.Email == _settings.Email);

                if (exists)
                {
                    _logger.LogInformation("Admin already exists");
                    return;
                }

                var admin = new User
                {
                    Name = "Admin",
                    Email = _settings.Email,
                    PasswordHash = PasswordHelper.Hash(_settings.Password),
                    Role = Roles.Admin.ToString(),
                };

                _db.Users.Add(admin);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Admin user seeded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding admin user");
                throw;
            }
        }
    }
}