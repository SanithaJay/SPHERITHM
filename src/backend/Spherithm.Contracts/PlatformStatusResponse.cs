namespace Spherithm.Contracts;

public sealed record PlatformStatusResponse(
    string Status,
    string Application,
    string Version,
    DateTime TimestampUtc
);
