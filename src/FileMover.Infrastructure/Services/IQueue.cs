namespace FileMover.Infrastructure.Services;

public interface IQueue
{
    public void Enqueue(Func<Task> job);
    public void Stop();
}
