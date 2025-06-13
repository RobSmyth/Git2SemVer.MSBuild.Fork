// ReSharper disable InconsistentNaming
namespace NoeticTools.Git2SemVer.Core.Diagnostics;

[DiagnosticCode]
public class GSV001 : DiagnosticCodeBase
{
    public GSV001(string fileName)
        : base(id: 1,
               description: """
                            This occurs when a project launches the solution versioning project to build the solution versioning information
                            but, after a waiting, the file still does not exist.
                            This has been know to occur when the build host has insufficient cores available to launch another project build from within a project.
                            """,
               resolution: """
                           Try increasing the number of cores available to the build host and try rebuilding again.
                           
                           Check build logs for if the solution versioning project did build and if it failed to build.
                           
                           If the problem is an error in the solution versioning project, you can delete the `.git2semver` folder in the solution
                           versioning project's folder and build the solution versioning project alone.
                           This will help diagnose the problem.
                           """,
               message: "Waited but couldn't find file '{0}'.",
               messageArgs: fileName)
    {
    }
}