namespace FileMover.Domain.Files
{
    public class FileMovement
    {
        public string Src { get; init; }
        public string Dest { get; private set; }

        public FileMovement(string src, string dest)
        {
            Src = src;
            Dest = dest;
        }

    }
}
