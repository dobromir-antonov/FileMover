using FileMover.Domain.Files;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace FileMover.Infrasturcutre.Services;

public class FileMovementQueue : IFileMovementQueue
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<FileMovement>> _fileMovementGroups;
    private readonly ILogger<FileMovementQueue> _logger;
    private readonly int _maxDegreeOfParallelism;

    public FileMovementQueue(ILogger<FileMovementQueue> logger)
    {
        _fileMovementGroups = new();
        _logger = logger;
        _maxDegreeOfParallelism = Environment.ProcessorCount / 2;
    }


    public void EnqueueFilesToMove(string srcFolder, string destFolder)
    {
        var fileGroups = Directory
            .GetFiles(srcFolder)
            .Select(filePath => new FileInfo(filePath))
            .GroupBy(file => file.Extension)
            .Select(grp => new FileMovementGroup(
                name: grp.Key, 
                files: grp
                    .Select(file => new FileMovement(
                        src: file.FullName, 
                        dest: $"{destFolder}/{file.Name}"))
                    .ToList())
            );

        foreach (var group in fileGroups) 
        {
            EnqueueFilesToMove(group.Name, group.Files);
        }
    }

    public async Task ProcessAllQueuedFilesAsync(CancellationToken cancellationToken)
    {
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = _maxDegreeOfParallelism,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(_fileMovementGroups.Keys, parallelOptions, async (group, cancellationToken) =>
        {
            if (_fileMovementGroups.TryGetValue(group, out var queue))
            {
                _logger.LogInformation("Files Count {Count} in Files Group {Group}", queue.Count, group);


                while (queue.TryDequeue(out var fileMovement))
                {
                    try
                    {
                        _logger.LogInformation("Move file from '{Src}' to '{Dest} on threadId {ThreadId}'", fileMovement.Src, fileMovement.Dest, Environment.CurrentManagedThreadId);
                        await FileHelper.MoveAsync(fileMovement.Src, fileMovement.Dest, cancellationToken: cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "'{Src}' file was not moved!", fileMovement.Src);
                    }
                }
            }
        });
    }


    private void EnqueueFilesToMove(string groupName, List<FileMovement> fileMovements)
    {
        _logger.LogInformation("Enqueue Files: {Files}", string.Join(", ", fileMovements.Select(x => x.Src)));

        _fileMovementGroups.AddOrUpdate(
            key: groupName,
            addValue: new(fileMovements),
            updateValueFactory: (key, queue) =>
            {
                foreach (var file in fileMovements)
                    queue.Enqueue(file);

                return queue;
            }
        );
    }
}
