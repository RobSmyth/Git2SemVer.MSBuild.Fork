using NativeLibraryLoader;
using System.Runtime.InteropServices;

namespace NoeticTools.Git2SemVer.MSBuild.LibGit2Interop
{
    internal class LibGit2NativeLibraryPathResolver : PathResolver
    {
        public override IEnumerable<string> EnumeratePossibleLibraryLoadTargets(string name)
        {
            var basePath = Path.GetDirectoryName(GetType().Assembly.Location)!;
            var fullPath = Path.Combine(basePath, GetRuntimePath(), name);
            Console.WriteLine($"=== Path = {fullPath}");
            return [fullPath];
        }

        private static string GetRuntimePath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "runtimes/win-x64/native";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "runtimes/linux-x64/native";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "runtimes/osx-x64/native";
            }

            throw new PlatformNotSupportedException($"The build host's platform '{RuntimeInformation.OSDescription}' is not supported. Windows, Linux, and OSX are supported");
        }
    }
}
