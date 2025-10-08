using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace library_system.Infrastructure.Persistence.Context
{
    public class ChallengeMotoConnectContextFactory : IDesignTimeDbContextFactory<ChallengeMotoConnectContext>
    {
        public ChallengeMotoConnectContext CreateDbContext(string[] args)
        {
            var environmentName = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Api"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var builder = new DbContextOptionsBuilder<ChallengeMotoConnectContext>();
            var connectionString = configuration.GetConnectionString("Oracle");

            builder.UseOracle(connectionString);

            return new ChallengeMotoConnectContext(builder.Options);
        }
    }
}

