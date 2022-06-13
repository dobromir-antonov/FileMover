using FileMover.Domain.Commands;
using FileMover.Domain.Files;
using FileMover.Infrastructure.Helpers;
using FileMover.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace FileMover.Infrasturcutre.Services;

public class FileMovementQueue : IFileMovementQueue
{
    private readonly Dictionary<string, IQueue> _queues;
    private readonly ILogger<FileMovementQueue> _logger;

    public FileMovementQueue(ILogger<FileMovementQueue> logger)
    {
        _queues = new();
        _logger = logger;
    }


    public void Enqueue(MoveCommand cmd, CancellationToken cancellationToken)
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
            EnqueueFilesByType(grp.FileType, grp.Files, cancellationToken);
        }
    }

    private void EnqueueFilesByType(string fileType, IEnumerable<FileMovement> files, CancellationToken cancellationToken)
    {
        if (!_queues.TryGetValue(fileType, out var queue))
        {
            queue = new BlockQueue(cancellationToken);
            _queues.TryAdd(fileType, queue);
        }

        foreach (var file in files)
        {
            queue.Enqueue(() => MoveFileAsync(file, cancellationToken));
        }
    }

    private async Task MoveFileAsync(FileMovement file, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("move {file}, thread: {thread}", file.FileName, Environment.CurrentManagedThreadId);
            await FileHelper.MoveFileAsync(file.Src, file.Dest, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("While moving file {file}, an error has occurred. {error}", file.FileName, ex.Message);
        }
    }

}
