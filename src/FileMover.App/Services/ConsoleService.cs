using FileMover.Domain.Commands;
using FileMover.Infrastructure.Helpers;
using FileMover.Infrasturcutre.Services;
using Microsoft.Extensions.Hosting;

namespace FileMover.Services;

public class ConsoleService
{
    private readonly IFileMovementQueue _fileQueue;
    private readonly CancellationToken _cancellationToken;

    public ConsoleService(
        IFileMovementQueue fileQueue,
        IHostApplicationLifetime applicationLifetime
        )
    {
        _fileQueue = fileQueue;
        _cancellationToken = applicationLifetime.ApplicationStopping;
    }


    public void StartUserInputMonitoring()
    {
        Task.Run(() => StartUserInputLoop());
    }


    private void StartUserInputLoop()
    {
        ICommand? command;
        bool stop = false;

        while (!_cancellationToken.IsCancellationRequested && !stop)
        {
            Console.Write("Please, enter a command:");

            var input = Console.ReadLine()!;
            command = CommandHelper.Parse(input);

            switch (command)
            {
                case MoveCommand cmd:
                    ProcessMoveCommand(cmd);
                    break;

                case ExitCommand:
                    stop = true;
                    Console.WriteLine("Exit");
                    break;

                default:
                    Console.WriteLine($"'{input}' is not a valid command!");
                    break;
            }
        }
    }


    private void ProcessMoveCommand(MoveCommand moveCommand)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(moveCommand.SrcFolder))
                throw new ArgumentException("Source folder is required!");

            if (string.IsNullOrWhiteSpace(moveCommand.DestFolder))
                throw new ArgumentException("Destination folder is required!");

            if (!Directory.Exists(moveCommand.SrcFolder))
                throw new ArgumentException("Source folder does not exist!");

            if (!Directory.Exists(moveCommand.DestFolder))
                throw new ArgumentException("Destination folder does not exist!");

            _fileQueue.EnqueueFilesToMove(moveCommand.SrcFolder, moveCommand.DestFolder);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

}
