using System.Threading.Channels;

namespace FileMover.Infrastructure.Services;

public class ChannelQueue
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

    public async ValueTask EnqueueAsync(Func<Task> function)
    {
        await _writer.WriteAsync(function, _cancellationToken);
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
