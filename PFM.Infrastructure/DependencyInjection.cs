using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PFM.Application.Options;
using PFM.Application.Services;
using PFM.Domain.Interfaces;
using PFM.Infrastructure.Persistence;
using PFM.Infrastructure.Persistence.Repositories;
using PFM.Infrastructure.Services;

namespace PFM.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // 1) Register DbContext
            services.AddDbContext<AppDbContext>(opts =>
                opts.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.Configure<AutoCategorizationOptions>(
                configuration.GetSection("AutoCategorization"));

            // 2) Wire up repositories
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ITransactionReadRepository, TransactionRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ITransactionSplitRepository, TransactionSplitRepository>();
            services.AddScoped<ICategoryReadRepository, CategoryRepository>();
            services.AddScoped<IAutoCategorizationService, AutoCategorizationService>();

            return services;
        }
    }
}
