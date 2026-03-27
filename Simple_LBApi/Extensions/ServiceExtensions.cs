
using Simple_LBApi.Seeders;

namespace Simple_LBApi.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddAppServices(
            this IServiceCollection services,
            IConfiguration config)
        {
            // Bind Admin Settings
            services.Configure<AdminSettings>(
                config.GetSection("AdminSettings"));

            // Register Seeder
            services.AddScoped<AdminSeeder>();

            return services;
        }
    }
}