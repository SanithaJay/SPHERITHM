using Spherithm.Domain.Entities;

namespace Spherithm.Application.Datasets.Abstractions;

public interface IDatasetRepository
{
    Task AddAsync(
        Dataset dataset,
        CancellationToken cancellationToken = default);
}