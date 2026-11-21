using Spherithm.Application.Datasets.Abstractions;
using Spherithm.Domain.Entities;

namespace Spherithm.Infrastructure.Persistence;

public sealed class DatasetRepository : IDatasetRepository
{
    private readonly SpherithmDbContext _dbContext;

    public DatasetRepository(SpherithmDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        Dataset dataset,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Datasets.AddAsync(dataset, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}