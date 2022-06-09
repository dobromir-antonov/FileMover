using FileMover.Domain.Files;
using System.Threading.Tasks.Dataflow;

namespace FileMover.Infrasturcutre.Services;

public class FileMovementQueueNew
{
    private readonly ActionBlock<string> _jobs;

    public FileMovementQueueNew()
    {
       
    }


    public void Enqueue(string job)
    {
        _jobs.Post(job);
    }

    public void Stop()
    {
    }

    private void OnHandlerStart()
    {
       
    }




}
