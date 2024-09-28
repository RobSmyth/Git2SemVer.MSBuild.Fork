---
uid: concepts
---
![](../Images/Git2SemVer_banner_840x70.png)

# Concepts

The Git2SemVer NuGet package is added to a project. 
It is a MSBuild task that inserts itself in the project's build sequence and before versions are generated.

![](../Images/MSBuild_tasks_01.png)

The object shown above are:

|    | Description |
|:-- |:-- |
| [C# script file](xref:csharp-script) | The code to customise the output. |
| [Build Host](xref:NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.Builders.IVersioningContext.Host) | Created by Git2SemVer to model the build host/build system. |
