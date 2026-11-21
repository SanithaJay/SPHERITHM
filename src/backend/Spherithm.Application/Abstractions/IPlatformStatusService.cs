using Spherithm.Contracts;

namespace Spherithm.Application.Abstractions;

public interface IPlatformStatusService
{
    PlatformStatusResponse GetStatus();
}
