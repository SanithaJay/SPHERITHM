using Spherithm.Application.Datasets.Abstractions;
using Spherithm.Application.Datasets.Commands;
using Spherithm.Domain.Entities;

namespace Spherithm.Application.Tests;

public class RegisterDatasetHandlerTests
{
    private const string ValidChecksum =
        "a3f5c8d1e2b4a697a3f5c8d1e2b4a697a3f5c8d1e2b4a697a3f5c8d1e2b4a697";

    [Fact]
    public async Task HandleAsync_WithValidCommand_CreatesAndSavesDataset()
    {
        var repository = new FakeDatasetRepository();
        var handler = new RegisterDatasetHandler(repository);

        var command = new RegisterDatasetCommand(
            "observation.fits",
            "FITS",
            1024,
            ValidChecksum);

        var datasetId = await handler.HandleAsync(command);

        Assert.NotEqual(Guid.Empty, datasetId);
        Assert.NotNull(repository.SavedDataset);
        Assert.Equal(datasetId, repository.SavedDataset.Id);
        Assert.Equal("observation.fits", repository.SavedDataset.FileName);
        Assert.Equal("FITS", repository.SavedDataset.Format);
        Assert.Equal(1024, repository.SavedDataset.SizeInBytes);
    }

    [Fact]
    public async Task HandleAsync_WithInvalidFormat_ThrowsException()
    {
        var repository = new FakeDatasetRepository();
        var handler = new RegisterDatasetHandler(repository);

        var command = new RegisterDatasetCommand(
            "observation.fits",
            "CSV",
            1024,
            ValidChecksum);

        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.HandleAsync(command));

        Assert.Null(repository.SavedDataset);
    }

    private sealed class FakeDatasetRepository : IDatasetRepository
    {
        public Dataset? SavedDataset { get; private set; }

        public Task AddAsync(
            Dataset dataset,
            CancellationToken cancellationToken = default)
        {
            SavedDataset = dataset;
            return Task.CompletedTask;
        }
    }
}