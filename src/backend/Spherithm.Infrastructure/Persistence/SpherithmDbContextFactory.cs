
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Spherithm.Infrastructure.Persistence;

public sealed class SpherithmDbContextFactory
    : IDesignTimeDbContextFactory<SpherithmDbContext>
{
    public SpherithmDbContext CreateDbContext(string[] args)
    {
        var apiAssembly = Assembly.Load("Spherithm.Api");

        var configuration = new ConfigurationBuilder()
            .AddUserSecrets(apiAssembly)
            .Build();

        var connectionString =
            configuration.GetConnectionString("SpherithmMigration")
            ?? throw new InvalidOperationException(
                "Migration connection string is missing.");

        var optionsBuilder =
            new DbContextOptionsBuilder<SpherithmDbContext>();

        optionsBuilder.UseMySQL(connectionString);

        return new SpherithmDbContext(optionsBuilder.Options);
    }
}
