namespace NoeticTools.Git2SemVer.Core.Tools.DotnetCli;

public interface ISolutionCommands
{
    /// <summary>
    ///     Create a new solution (sln) with the same name as the folder.
    /// </summary>
    void New();

    /// <summary>
    ///     Create a new solution (sln) with the given name.
    /// </summary>
    /// <remarks>
    ///     Equivalent to
    ///     <see href="https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-sln#create-a-solution-file">
    ///         dotnet CLI create
    ///         a solution file
    ///     </see>
    ///     .
    /// </remarks>
    void New(string solutionName);
}