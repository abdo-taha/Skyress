using Microsoft.Extensions.DependencyInjection;
using Skyress.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Skyress.Application.Contracts.Persistence;
using Skyress.Infrastructure.Repository;
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
            return services;
        }
    }
}
