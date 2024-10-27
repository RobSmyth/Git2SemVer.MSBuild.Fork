---
uid: getting-started
---
![](../Images/Git2SemVer_banner_840x70.png)

# Getting Started

## Prerequisites

Git2SemVer requires:

* `git` CLI to be executable from any project directory.
* `dotnet` CLI to be executable from any project directory.

Known compatibility:

* `dotnet.exe` `8.0.403` or later.
* `git` `2.41.0` or later.
* Windows 11
* Ubuntu 20.04 LTS

## Installing


To setup a solution to use Git2SemVer solution versioning, first install the dotnet tool `Git2SemVer.Tool`:

```winbatch
dotnet tool install --global NoeticTools.Git2SemVer.Tool
```

> [!NOTE]
> Git2SemVer.Tool is only required for solution setup.

Then, in the solution's directory, run:

```winbatch
Git2SemVer add
```

You will be prompted with a few options and then setup is done.

Try building the solution, all projects will be automatically versioned using [Git2SemVer's default versioning](xref:default-versioning).

> [!TIP]
> Git2SemVer outputs the generated informational version to the compiler's output.


## Quick Start

If you have not installed Git2Sember do it now. Instructions [here]((#installing)).
Then, if your happy with [Git2SemVer's default versioning](xref:default-versioning), you have nothing more to do.

To customise the versioning open the file `Git2SemVer.csx` in the `SolutionVersioning` (or the name given during setup) project and add:

```csharp
Log.LogInfo("Hello world - my first Git2SemVer C# script is born.");
```

Rebuild and you will the message `Hello world - my first After Burner C# script is born.` in the compiler's output.

That was using C# script globals. `Log` is a global property. 
Another way is to use a context instance exposted by the `VersionContext` property:

```csharp
var context = VersioningContext.Instance!;

context.Logger.LogInfo("Hello world - my second Git2SemVer C# script is born.");
```

`VersioningContext.Instance` provides the global properties as a context object. It provides a better coding experience.
We will use the context object but the same can be done using the global properties.

> [!TIP]
> Git2SemVer has loaded common libraries and has setup implied using namespaces.
> Using statements may added to the file for more advanced editing.

As a simple example of seting assembly, file, informational, as well as package versions, replace the code in the csx file with:

```csharp
var context = VersioningContext.Instance!;
context.Logger.LogInfo("Running demo Git2SemVer customisation script.");

context.Outputs.SetAllVersionPropertiesFrom(SemVersion.ParseFrom("1.2.3-Demo.999+ASimpleDemoScriptVersion"));
```
 
 This will give you:

 <pre>
 Informational version:     1.2.3-Demo.999+ASimpleDemoScriptVersion
 Version:                   1.2.3-Demo.999
 AssemblyVersion:           1.2.3.0
 FileVersion:               1.2.3.0
 Build system version:      1.2.3-Demo.999
 Prereleaase label:         Demo
 Is in initial development: false</pre>

To demonstate reading Git2SemVer properties and overriding/customising everything replace the code in the csx file with:

[!code-csharp[](CsxDemos/ForceProperties4.csx)]


## Configuring build system

Your build system may need to be configured to make the build number available to Git2SemVer. See [Build Hosts](xref:build-hosts).