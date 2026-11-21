using Spherithm.Application.Datasets.Abstractions;
using Spherithm.Domain.Entities;

namespace Spherithm.Application.Datasets.Commands;

public sealed class RegisterDatasetHandler
{
    private readonly IDatasetRepository _repository;

    public RegisterDatasetHandler(IDatasetRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> HandleAsync(
        RegisterDatasetCommand command,
        CancellationToken cancellationToken = default)
    {
        var dataset = new Dataset(
            command.FileName,
            command.Format,
            command.SizeInBytes,
            command.ChecksumSha256);

        await _repository.AddAsync(dataset, cancellationToken);

        return dataset.Id;
    }
}