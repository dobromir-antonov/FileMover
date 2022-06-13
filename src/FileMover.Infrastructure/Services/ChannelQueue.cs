using System.Threading.Channels;

namespace FileMover.Infrastructure.Services;

public class ChannelQueue : IQueue
{
    private readonly ChannelWriter<Func<Task>> _writer;
    private readonly ChannelReader<Func<Task>> _reader;
    private readonly CancellationToken _cancellationToken;

    public ChannelQueue(CancellationToken cancellationToken)
    {
        var channel = Channel.CreateUnbounded<Func<Task>>(new UnboundedChannelOptions
        {
            SingleReader = true
        });

        _reader = channel.Reader;
        _writer = channel.Writer;
        _cancellationToken = cancellationToken;

        Task.Run(ReadMessagesAsync, cancellationToken);
    }

    public void Enqueue(Func<Task> job)
    {
        _writer.TryWrite(job);
    }

    public void Stop()
    {
        _writer.Complete();
    }

    private async Task ReadMessagesAsync()
    {
        await foreach (var function in _reader.ReadAllAsync(_cancellationToken))
        {
            await function.Invoke();
        }
    }
}
