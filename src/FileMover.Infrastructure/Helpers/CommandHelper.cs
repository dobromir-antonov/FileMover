using FileMover.Domain.Commands;
using FileMover.Infrastructure.Enums;

namespace FileMover.Infrastructure.Helpers;

public static partial class CommandHelper
{
    public static ICommand? Parse(string input)
    {
        var parts = input.Split(" ");
        var cmd = parts[0];

        switch (cmd)
        {
            case "move":
                var src = GetFolder(input, FolderType.Source);
                var dest = GetFolder(input, FolderType.Destination);
                return new MoveCommand(src, dest);

            case "exit":
                return new ExitCommand();

            default:
                return null;
        }
    }

    private static string GetFolder(string input, FolderType folderType)
    {
        var splitter = folderType switch
        {
            FolderType.Destination => "-d ",
            _ => "-s "
        };

        var parts = input.Split(splitter, StringSplitOptions.TrimEntries);

        if (parts.Length <= 1)
            return "";


        var result = parts[1]
            .Split(" ")[0]
            .Replace("'", "")
            .Replace("\"", "");

        return result;
    }

}

