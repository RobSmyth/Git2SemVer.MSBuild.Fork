using LibGit2Sharp;
using NativeLibraryLoader;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using NativeLibrary = NativeLibraryLoader.NativeLibrary;

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
        private const string LibGit2BaseName = "git2";

        private static string GetFileName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WriteLine("== WIN");//>>>
                return $"{LibGit2BaseName}-{LibGit2Sha}.dll"; 
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Console.WriteLine("== Linux");//>>>
                return $"lib{LibGit2BaseName}-{LibGit2Sha}.so";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Console.WriteLine("== OSX");//>>>
                return $"lib{LibGit2BaseName}-{LibGit2Sha}.dylib";
            }

            throw new PlatformNotSupportedException($"The build host's platform '{RuntimeInformation.OSDescription}' is not supported. Windows, Linux, and OSX are supported");
        }

        public LibGit2NativeLibrary() 
            : base(GetFileName(), LibraryLoader.GetPlatformDefaultLoader(), new LibGit2NativeLibraryPathResolver())
        {
            //if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            //{
            //    var nativeLibraryPath = "/opt/TeamCity/buildAgent/work/5310bb125709005e/Git2SemVer.MSBuild/bin/Release/netstandard2.0/runtimes/linux-x64/native/libgit2-3f4182d.so";
            //    if (!nativeLibraryPath.Equals(GlobalSettings.NativeLibraryPath))
            //    {
            //        GlobalSettings.NativeLibraryPath = nativeLibraryPath;
            //    }
            //}
            // "/opt/TeamCity/buildAgent/work/5310bb125709005e/Git2SemVer.MSBuild/bin/Release/netstandard2.0/runtimes/linux-x64/native/libgit2-3f4182d.so"
        }
    }
}
