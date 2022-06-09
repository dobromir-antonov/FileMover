namespace FileMover.Domain.Commands;

public class MoveCommand : ICommand
{
    public string SrcFolder { get; private set; }
    public string DestFolder { get; private set; }

    public MoveCommand(string srcPath, string destPath)
    {
        SrcFolder = srcPath;
        DestFolder = destPath;
    }

    public void Validate()
    { 
    
    }
}
