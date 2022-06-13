using FileMover.Domain.Commands;

namespace FileMover.Infrasturcutre.Services;
public interface IFileMovementQueue
{
    void Enqueue(MoveCommand cmd, CancellationToken cancellationToken);
}
