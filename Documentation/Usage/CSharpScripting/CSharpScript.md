---
uid: csharp-script
---
![](../../Images/Git2SemVer_banner_840x70.png)

# C# script

## Location

On each build, Git2SemVer loads and executes a C# script (CS-Script) file.
MSBuild property `Git2SemVer_ScriptPath` provides the path to this C# script file.
It defaults to the file `Git2SemVer.csx` in the project folder.

The CS-Script file may be empty but if the file is not found the build fails.

Examples setting script path:

# [dotnet command line](#tab/dotnet)

```winbatch
  dotnet build -p:Git2SemVer_ScriptPath=Build/MyVersioning.csx
```

# [MSBuild csproj file](#tab/msbuild)

```xml
<Git2SemVer_ScriptPath>$(MSBuildProjectDirectory)/Build/MyVersioning.csx</Git2SemVer_ScriptPath>
```

---

## Globals and script runner instance

[VersioningContext](xref:NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.Builders.Scripting.IVersioningContext) instance properties are exposed to the script's global context.
This makes the following properties available as globals:

| Property                      | Description   |
|:---                           |:---           |
| [Host](xref:NoeticTools.Git2SemVer.MSBuild.Framework.BuildHosting.IBuildHost)               | A build host instance modeling the detected, or set, current host type. Provides a `BuildNumber` property.   |
| [Inputs](xref:NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.IVersionGeneratorInputs) | The task's MSBuild input property values. Includes a property for optional script arguments.          |
| [Logger](xref:NoeticTools.Common.Logging.ILogger)                                           | An MSBuild logger. Logging an error will cause the build to fail.                                     |
| [MsBuildGlobalProperties](xref:NoeticTools.MSBuild.Tasking.MSBuildGlobalProperties)         | Harvested MSBuild global properties for optional script use.                                          |
| [Outputs](xref:NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.IVersionOutputs)        | Outputs that the script may optionally write to. These will be available to other MSBuild tasks.      |

[VersioningContext](xref:NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.Builders.Scripting.VersioningContext) also provides a static property `Instance` with the context's instance.

If using a C# script editing project for syntax highlighting and itellisence (highly recommended), 
intellisens does not recognise global property types.
For a better editing experence work obtain the runner from static property instead. 
It is the same instance but will be easier to edit.

Script examples logging a message using `Logger`:

# Using global properties

```csharp
Logger.LogInfo("Hello world.")
```

# Using runner global instance (recommened)

```csharp
var context = VersioningContext.Instance!;

context.Logger.LogInfo.LogImportantMessage("Hello world.")
```

---

## Using classes

The C# script has a global context and a class context. To use classes the runner instance needs to be passed to the class.

> [!TIP]
> Consider using the runner global instance instead.
> The versioning work is not complex, using the runner in the global context may be  simpler option.

Example:

```csharp
var context = VersioningContext.Instance!;

new MyVersioningClass(context).Run();

public class MyVersioningClass
{
    private IVersionContext _context;

    public MyVersioningClass(IVersionContext context)
    {
        _context = context;
    }

    public void Run()
    {
        _context.Logger.LogImportantMessage("Hello world from my versioning class.")
    }
}
```

> [!NOTE]
> Global context properties and variables may not be available from within a class context.

---


