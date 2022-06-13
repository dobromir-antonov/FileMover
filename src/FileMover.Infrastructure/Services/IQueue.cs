namespace FileMover.Infrastructure.Services;

public interface IQueue
{
    public Task EnqueueAsync(Func<Task> job);
    public void Stop();
}
