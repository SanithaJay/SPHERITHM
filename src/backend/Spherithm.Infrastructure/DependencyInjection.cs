
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spherithm.Application.Datasets.Abstractions;
using Spherithm.Infrastructure.Persistence;

namespace Spherithm.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SpherithmDatabase")
            ?? throw new InvalidOperationException(
                "Database connection string 'SpherithmDatabase' is missing.");

        services.AddDbContext<SpherithmDbContext>(options =>
            options.UseMySQL(connectionString));

        services.AddScoped<IDatasetRepository, DatasetRepository>();

        return services;
    }
}
