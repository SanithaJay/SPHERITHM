namespace Spherithm.Domain.Entities;

public sealed class Dataset
{
    public Guid Id { get; private set; }

    public string FileName { get; private set; } = string.Empty;

    public string Format { get; private set; } = string.Empty;

    public long SizeInBytes { get; private set; }

    public string ChecksumSha256 { get; private set; } = string.Empty;

    public DateTime UploadedAtUtc { get; private set; }

    private Dataset()
    {
    }

    public Dataset(
        string fileName,
        string format,
        long sizeInBytes,
        string checksumSha256)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required.", nameof(fileName));

        if (string.IsNullOrWhiteSpace(format))
            throw new ArgumentException("File format is required.", nameof(format));

        if (!string.Equals(format.Trim(), "FITS", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Only FITS format is currently supported.", nameof(format));

        if (!string.Equals(Path.GetFileName(fileName), fileName, StringComparison.Ordinal))
            throw new ArgumentException("File name must not contain a directory path.", nameof(fileName));

        var extension = Path.GetExtension(fileName);

        if (!string.Equals(extension, ".fits", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(extension, ".fit", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Only .fits and .fit file extensions are supported.", nameof(fileName));
        }

        if (sizeInBytes <= 0)
            throw new ArgumentOutOfRangeException(nameof(sizeInBytes), "File size must be greater than zero.");

        if (string.IsNullOrWhiteSpace(checksumSha256) ||
            checksumSha256.Length != 64 ||
            !checksumSha256.All(Uri.IsHexDigit))
        {
            throw new ArgumentException("A valid SHA-256 checksum is required.", nameof(checksumSha256));
        }

        Id = Guid.NewGuid();
        FileName = fileName;
        Format = "FITS";
        SizeInBytes = sizeInBytes;
        ChecksumSha256 = checksumSha256.ToLowerInvariant();
        UploadedAtUtc = DateTime.UtcNow;
    }
}