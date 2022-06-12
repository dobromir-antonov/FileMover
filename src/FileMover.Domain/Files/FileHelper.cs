namespace FileMover.Domain.Files;

public static class FileHelper
{
    public static async Task MoveFileAsync(string src, string dest, int bufferSize = 4096, CancellationToken cancellationToken = default)
    {
        using var srcStream = new FileStream(src, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);
        using var destStream = new FileStream(dest, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);
        await srcStream.CopyToAsync(destStream, cancellationToken);
    }
}

