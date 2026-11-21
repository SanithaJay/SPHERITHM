using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spherithm.Application.Abstractions;
using Spherithm.Infrastructure.Persistence;

namespace Spherithm.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    private readonly IPlatformStatusService _statusService;
    private readonly SpherithmDbContext _dbContext;
    private readonly ILogger<HealthController> _logger;

    public HealthController(
        IPlatformStatusService statusService,
        SpherithmDbContext dbContext,
        ILogger<HealthController> logger)
    {
        _statusService = statusService;
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var status = _statusService.GetStatus();

        return Ok(status);
    }

    [HttpGet("database")]
    
    public async Task<IActionResult> CheckDatabase(
    CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Database.OpenConnectionAsync(cancellationToken);

            await _dbContext.Database.CloseConnectionAsync();

            return Ok(new
            {
                Status = "Connected",
                Database = "spherithm",
                Provider = "MySQL"
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Database connectivity check failed.");

            return StatusCode(503, new
            {
                Status = "Unavailable",
                Database = "spherithm",
                Provider = "MySQL"
            });
        }
    }
}
