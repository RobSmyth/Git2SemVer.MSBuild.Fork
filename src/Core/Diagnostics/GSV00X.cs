// ReSharper disable UnusedType.Global
// ReSharper disable InconsistentNaming
namespace NoeticTools.Git2SemVer.Core.Diagnostics;

//[DiagnosticCode]
public sealed class GSV00X : DiagnosticCodeBase
{
    public GSV00X(string sampleArg)
        : base(id: 9999,
               subcategory: "Versioning",
               description: """
                            This occurs when ...
                            """,
               resolution: """
                           If there ...
                           """,
               message: "The required build script file '{0}' was not found.",
               messageArgs: sampleArg)
    {
    }
}