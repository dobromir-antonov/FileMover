using FileMover.Domain.Commands;

namespace FileMover.Infrasturcutre.Services;
public interface IFileMovementQueue
{
    Task EnqueueAsync(MoveCommand cmd, CancellationToken cancellationToken);
}
