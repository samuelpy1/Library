using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using library_system.Infrastructure.Persistence.Context;
using library_system.Domain.Interfaces;
using library_system.Infrastructure.Persistence.Repositories;

namespace library_system.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ChallengeMotoConnectContext>(options =>
            {
                options.UseOracle(configuration.GetConnectionString("Oracle"));
            });

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            return services;
        }
    }
}


