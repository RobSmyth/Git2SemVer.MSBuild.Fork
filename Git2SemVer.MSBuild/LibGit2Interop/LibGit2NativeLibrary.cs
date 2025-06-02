using NativeLibraryLoader;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NoeticTools.Git2SemVer.MSBuild.LibGit2Interop
{
    internal class LibGit2NativeLibrary : NativeLibrary
    {
        /// <summary>
        ///     The git sha that the used 'LibGit2Sharp.NativeBinaries' package was compiled against.
        /// </summary>
        /// <remarks>
        ///     Must match both the filenames used in both the 'LibGit2Sharp.NativeBinaries' and Git2SemVer runtimes folders.
        /// </remarks>
        private const string LibGit2Sha = "3f4182d";

        private static string GetFileName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return $"libgit2-{LibGit2Sha}.dll";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return $"libgit2-{LibGit2Sha}.so";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return $"git2-{LibGit2Sha}.dylib";
            }

            throw new PlatformNotSupportedException($"The build host's platform '{RuntimeInformation.OSDescription}' is not supported. Windows, Linux, and OSX are supported");
        }

        public LibGit2NativeLibrary() 
            : base(GetFileName(), LibraryLoader.GetPlatformDefaultLoader(), new LibGit2NativeLibraryPathResolver())
        {
        }
    }
}
