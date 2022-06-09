
using FileMover.Infrasturcutre.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FileMover.Services;
public sealed class BackgroundFileService : BackgroundService
{
    private readonly IFileMovementQueue _fileQueue;
    private readonly ILogger<BackgroundFileService> _logger;
    private readonly int _delay;

    public BackgroundFileService(
        IFileMovementQueue fileQueue,
        IConfiguration configuration,
        ILogger<BackgroundFileService> logger)
    {
        _fileQueue = fileQueue;
        _logger = logger;
        _delay = configuration.GetValue<int>("FileProcessor:DelayInMiliseconds");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("--- FILE PROCESSOR RUNS ---");

            try
            {
                await _fileQueue.ProcessAllQueuedFilesAsync(stoppingToken);
                await Task.Delay(_delay, stoppingToken);
            }
            catch (OperationCanceledException) { } // prevent throwing, if stoppingToken was signaled
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error occured executing file processing.");
            }
        }
    }
}
