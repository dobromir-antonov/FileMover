namespace FileMover.Domain.Files
{
    public class FileMovement
    {
        public string Src { get; init; }
        public string Dest { get; private set; }
        public string FileName { get; private set; }

        public FileMovement(string src, string dest, string name)
        {
            Src = src;
            Dest = dest;
            FileName = name;
        }

    }
}
