namespace FileMover.Domain.Files
{
    public class FileMovementGroup
    {
        public string Name { get; init ; }
        public List<FileMovement> Files { get; init; }

        public FileMovementGroup(string name, List<FileMovement> files)
        {
            Name = name;
            Files = files;
        }
    }
}
