using Microsoft.Extensions.DependencyInjection;
using Skyress.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Skyress.Application.Contracts.Persistence;
using Skyress.Infrastructure.Repository;
using Skyress.Infrastructure.Services;
using Skyress.Application.Auth.Contracts.Persistence;
using Skyress.Application.Auth.Contracts.Services;
using Microsoft.EntityFrameworkCore;


namespace Skyress.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrasructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SkyressDb");
            services.AddDbContext<SkyressDbContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<ITagAssignmentRepository, TagAssignmentRepository>();
            services.AddScoped<ITodoRepository, TodoRepository>();
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.Configure<JwtSettings>(opts =>
            {
                var section = configuration.GetSection("Jwt");
                opts.Issuer = section["Issuer"] ?? "Skyress";
                opts.Audience = section["Audience"] ?? "SkyressAPI";
                opts.AccessTokenExpiryMinutes = int.TryParse(section["AccessTokenExpiryMinutes"], out var atm) ? atm : 15;
                opts.RefreshTokenExpiryDays = int.TryParse(section["RefreshTokenExpiryDays"], out var rtd) ? rtd : 7;
                opts.SecretKey = section["SecretKey"] ?? string.Empty;
            });
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            return services;
        }
    }
}
