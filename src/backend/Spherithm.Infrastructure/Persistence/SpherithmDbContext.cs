using Microsoft.EntityFrameworkCore;
using Spherithm.Domain.Entities;

namespace Spherithm.Infrastructure.Persistence;

public sealed class SpherithmDbContext : DbContext
{
    public SpherithmDbContext(
        DbContextOptions<SpherithmDbContext> options)
        : base(options)
    {
    }

    public DbSet<Dataset> Datasets => Set<Dataset>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Dataset>(entity =>
        {
            entity.ToTable("datasets");

            entity.HasKey(dataset => dataset.Id);

            entity.Property(dataset => dataset.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            entity.Property(dataset => dataset.FileName)
                .HasColumnName("file_name")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(dataset => dataset.Format)
                .HasColumnName("format")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(dataset => dataset.SizeInBytes)
                .HasColumnName("size_in_bytes")
                .IsRequired();

            entity.Property(dataset => dataset.ChecksumSha256)
                .HasColumnName("checksum_sha256")
                .HasMaxLength(64)
                .IsRequired();

            entity.Property(dataset => dataset.UploadedAtUtc)
                .HasColumnName("uploaded_at_utc")
                .IsRequired();
        });
    }
}