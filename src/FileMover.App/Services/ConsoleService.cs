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
        Task.Run(async () => await StartUserInputLoopAsync(_cancellationToken), _cancellationToken);
    }


    private async Task StartUserInputLoopAsync(CancellationToken cancellationToken)
    {
        ICommand? command;
        bool exit = false;

        while (!cancellationToken.IsCancellationRequested && !exit)
        {
            Console.Write("Please, enter a command:");

            var input = Console.ReadLine()!;
            command = CommandHelper.Parse(input);

            switch (command)
            {
                case MoveCommand cmd:
                    await ProcessMoveCommandAsync(cmd, cancellationToken);
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


    private async Task ProcessMoveCommandAsync(MoveCommand moveCommand, CancellationToken cancellationToken)
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

           await _fileQueue.EnqueueAsync(moveCommand, cancellationToken);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

}
