namespace FileMover.Infrasturcutre.Services;
public interface IFileMovementQueue
{
    void EnqueueFilesToMove(string srcFolder, string destFolder);
    Task ProcessAllQueuedFilesAsync(CancellationToken cancellationToken);
}
