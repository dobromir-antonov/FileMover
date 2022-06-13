namespace FileMover.Infrastructure.Helpers;
public static class FileHelper
{
    public static async Task MoveFileAsync(
        string src, string dest, int bufferSize = 4096, 
        bool overwrite = true, CancellationToken cancellationToken = default)
    {
        using var srcStream = new FileStream(src, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan | FileOptions.DeleteOnClose);
        using var destStream = new FileStream(dest, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);
        await srcStream.CopyToAsync(destStream, cancellationToken);
    }
}

