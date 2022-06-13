using FileMover.Domain.Commands;
using FileMover.Infrastructure.Helpers;
using FileMover.Infrasturcutre.Services;
using Microsoft.Extensions.Hosting;

namespace FileMover.Services;

public class ConsoleService : IConsoleService
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
        Task.Run(() => StartUserInputLoop(), _cancellationToken);
    }


    private void StartUserInputLoop()
    {
        ICommand? command;
        bool exit = false;

        while (!_cancellationToken.IsCancellationRequested && !exit)
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
                    exit = true;
                    Console.WriteLine("Exit");
                    break;

                default:
                    Console.WriteLine($"'{input}' is not a valid command!");
                    break;
            }
        }
    }


    public void ProcessMoveCommand(MoveCommand moveCommand)
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

            _fileQueue.Enqueue(moveCommand, _cancellationToken);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

}
