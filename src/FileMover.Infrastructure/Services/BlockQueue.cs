﻿using System.Threading.Tasks.Dataflow;

namespace FileMover.Infrastructure.Services;

public class BlockQueue : IQueue
{
    private readonly ActionBlock<Func<Task>> _queue;
    private readonly CancellationToken _cancellationToken;

    public BlockQueue(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        _queue = CreateQueue();
    }

    public Task EnqueueAsync(Func<Task> job)
    {
        return _queue.SendAsync(job, _cancellationToken);
    }

    public void Stop()
    {
        _queue.Complete();
    }

    private ActionBlock<Func<Task>> CreateQueue()
    {
        return new ActionBlock<Func<Task>>((job) => job.Invoke());
    }
}
