---
uid: csharp-script-namespaces
---

<div style="background-color:#944248;padding:0px;margin-bottom:0.5em">
  <img src="https://noetictools.github.io/Git2SemVer/Images/Git2SemVer_banner_840x70.png"/>
</div>


# Namespaces and references

## Namespaces

> [!TIP]
> At run-time (build-time) the C# scipt does not need `using` directives for the namespaces listed here.
> However using directives are necessary to get intellisence and syntax checking when editing
> a script in an IDE project.

The following namespaces are imported in the C# script's global context:

```
Semver
NuGet.Versioning
System
System.IO
System.Numerics
System.Linq
System.Globalization
System.Diagnostics
System.Collections
System.Collections.Generic
System.Runtime.Serialization
System.Text.RegularExpressions
System.Runtime.CompilerServices
System.Runtime.InteropServices
System.Diagnostics.CodeAnalysis
Microsoft.Extensions.Primitives
NoeticTools.MSBuild.Tasking
NoeticTools.Common.Tools.DotnetCli
NoeticTools.Git2SemVer.MSBuild.Tools.CI
NoeticTools.Git2SemVer.MSBuild.Framework.BuildHosting
NoeticTools.Common.Tools.Git
NoeticTools.Git2SemVer.MSBuild.Scripting
NoeticTools.Common.Logging
```

## References

The following assemblies are references in the C# script's context: 

```
Semver
NuGet.Versioning
System.Linq
System.Private.CoreLib
System.Text.RegularExpressions
System.Runtime.Numerics
Microsoft.Extensions.Primitives
NoeticTools.Common
NoeticTools.MSBuild.Tasking
NoeticTools.Git2SemVer.MSBuild
```

> [!NOTE]
> For the dotnet and Visual Studio compatibility references must be .NET Standard 2.0 compatible.
