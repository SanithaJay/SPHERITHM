using Spherithm.Domain.Entities;

namespace Spherithm.Domain.Tests;

public class DatasetTests
{
    private const string ValidChecksum =
        "a3f5c8d1e2b4a697a3f5c8d1e2b4a697a3f5c8d1e2b4a697a3f5c8d1e2b4a697";

    [Fact]
    public void Constructor_WithValidData_CreatesDataset()
    {
        var dataset = new Dataset(
            "observation.fits",
            "FITS",
            1024,
            ValidChecksum);

        Assert.NotEqual(Guid.Empty, dataset.Id);
        Assert.Equal("observation.fits", dataset.FileName);
        Assert.Equal("FITS", dataset.Format);
        Assert.Equal(1024, dataset.SizeInBytes);
        Assert.Equal(ValidChecksum, dataset.ChecksumSha256);
        Assert.NotEqual(default, dataset.UploadedAtUtc);
    }

    [Fact]
    public void Constructor_WithEmptyFileName_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Dataset("", "FITS", 1024, ValidChecksum));
    }

    [Fact]
    public void Constructor_WithEmptyFormat_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Dataset("observation.fits", "", 1024, ValidChecksum));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidFileSize_ThrowsException(long size)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Dataset("observation.fits", "FITS", size, ValidChecksum));
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-checksum")]
    [InlineData("a3f5")]
    public void Constructor_WithInvalidChecksum_ThrowsException(string checksum)
    {
        Assert.Throws<ArgumentException>(() =>
            new Dataset("observation.fits", "FITS", 1024, checksum));
    }
    [Fact]
    public void Constructor_WithNonFitsFormat_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Dataset("observation.fits", "CSV", 1024, ValidChecksum));
    }

    [Fact]
    public void Constructor_WithUnsupportedExtension_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Dataset("observation.txt", "FITS", 1024, ValidChecksum));
    }

    [Fact]
    public void Constructor_WithDirectoryPath_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Dataset(@"C:\Data\observation.fits", "FITS", 1024, ValidChecksum));
    }

    [Fact]
    public void Constructor_WithLowercaseFormat_NormalizesToFITS()
    {
        var dataset = new Dataset(
            "observation.fits",
            "fits",
            1024,
            ValidChecksum);

        Assert.Equal("FITS", dataset.Format);
    }
    [Fact]
    public void Constructor_WithFitExtension_CreatesDataset()
    {
        var dataset = new Dataset(
            "observation.fit",
            "FITS",
            1024,
            ValidChecksum);

        Assert.Equal("observation.fit", dataset.FileName);
    }

    [Fact]
    public void Constructor_WithUppercaseChecksum_NormalizesToLowercase()
    {
        var dataset = new Dataset(
            "observation.fits",
            "FITS",
            1024,
            ValidChecksum.ToUpperInvariant());

        Assert.Equal(ValidChecksum, dataset.ChecksumSha256);
    }
}