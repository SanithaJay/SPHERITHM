namespace Spherithm.Application.Datasets.Commands;

public sealed record RegisterDatasetCommand(
    string FileName,
    string Format,
    long SizeInBytes,
    string ChecksumSha256
);