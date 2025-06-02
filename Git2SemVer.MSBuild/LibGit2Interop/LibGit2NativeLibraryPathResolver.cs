using NativeLibraryLoader;
using System.Runtime.InteropServices;

namespace NoeticTools.Git2SemVer.MSBuild.LibGit2Interop
{
    internal class LibGit2NativeLibraryPathResolver : PathResolver
    {
        public override IEnumerable<string> EnumeratePossibleLibraryLoadTargets(string name)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return [Path.Combine("win-x64/native", name)];
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return [Path.Combine("linux-x64/native", name)];
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return [Path.Combine("osx-x64/native", name)];
            }

            throw new PlatformNotSupportedException($"The build host's platform '{RuntimeInformation.OSDescription}' is not supported. Windows, Linux, and OSX are supported");
        }
    }
}
