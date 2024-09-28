namespace NoeticTools.Common;

public static class FileSystemInfoExtensions
{
    public static bool IsDirectory(this FileSystemInfo info)
    {
        return (info.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
    }

    public static bool IsFile(this FileSystemInfo info)
    {
        return !info.IsDirectory();
    }
}