using Spherithm.Application.Abstractions;
using Spherithm.Contracts;

namespace Spherithm.Application.Services;

public sealed class PlatformStatusService : IPlatformStatusService
{
    public PlatformStatusResponse GetStatus()
    {
        return new PlatformStatusResponse(
            Status: "Healthy",
            Application: "SPHERITHM API",
            Version: "1.0.0",
            TimestampUtc: DateTime.UtcNow
        );
    }
}
