using FileMover.Domain.Commands;
using FileMover.Domain.Files;
using FileMover.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace FileMover.Infrasturcutre.Services;

public class FileMovementQueue : IFileMovementQueue
{
    private readonly Dictionary<string, ChannelQueue> _queues = new();
    private readonly ILogger<FileMovementQueue> _logger;

    public FileMovementQueue(ILogger<FileMovementQueue> logger)
    {
        _logger = logger;
    }


    public async Task EnqueueAsync(MoveCommand cmd, CancellationToken cancellationToken)
    {
        var filesByType = Directory
            .GetFiles(cmd.SrcFolder)
            .Select(filePath => new FileInfo(filePath))
            .GroupBy(file => file.Extension)
            .Select(grp => new
            {
                FileType = grp.Key,
                Files = grp.Select(f => new FileMovement(f.FullName, $"{cmd.DestFolder}/{f.Name}", f.Name))
            });

        foreach (var grp in filesByType)
        {
            await EnqueueFilesByTypeAsync(grp.FileType, grp.Files, cancellationToken);
        }
    }

    private async Task EnqueueFilesByTypeAsync(string fileType, IEnumerable<FileMovement> files, CancellationToken cancellationToken)
    {
        if (!_queues.TryGetValue(fileType, out var queue))
        {
            queue = new ChannelQueue(cancellationToken);
            _queues.TryAdd(fileType, queue);
        }

        foreach (var file in files)
        {
            await queue.EnqueueAsync(() => MoveFileAsync(file, cancellationToken));
        }
    }

    private async Task MoveFileAsync(FileMovement file, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("file: {file}, Thread: {thread}", file.FileName, Environment.CurrentManagedThreadId);
            await FileHelper.MoveFileAsync(file.Src, file.Dest, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

}
