---
_layout: landing
---
<style>
.featureDiv {
  font-size:1.2em;
}

table, tr, td {
  border:none !important;
}

a 
{
  text-decoration: none; 
}
</style>

![](Images/Git2SemVer_banner_840x70.png)

![Current Version](https://img.shields.io/nuget/vpre/NoeticTools.Git2SemVer)
[![Release Build](https://github.com/NoeticTools/Git2SemVer/actions/workflows/dotnet.yml/badge.svg)](https://github.com/NoeticTools/Git2SemVer/actions/workflows/dotnet.yml)


# Git2SemVer

<div style="margin-left:0px; margin-top:-5px; margin-bottom:35px; font-family:Calibri; font-size:1.3em;">
No limits .NET solution versioning.</div>

> [!NOTE]  
> This project is in early development and may undergo large changes
> before the first stable release 1.0.0. 
>
> Early testing and feedback would be great!

Git2SemVer provides automatic Semantic Versioning to .NET solutions and projects. 
It automatically versions both dotnet CLI and Visual Studio builds so both developers and build system builds are versioned.
It just happens using <a href="https://www.conventionalcommits.org/en/v1.0.0/">Conventional Commits</a> message elements and Git tagging of each release.

Git2SemVer includes a dotnet CLI tool (Git2SemVer.Tool) for simple solution setup. 
The setup configures add the Git2SemVer.MSBuild NuGet package to all solution projects.
With the versioning being done within the dotnet/MSBuild build no build system version generation steps are needed.

No complex configuration of branching strategy (like GitFlow or GitHubFlow) and completely customisable.
Git2SemVer determines Semantic Version from Git commit history and build host parameters and environment. 
It then executes an optional project C# script that can change any or all of the generated version information.

<br/>

## Why Use Git2SemVer?

<div style="margin:10px;">
 <table>
  <tr>
    <td>
        <div class="featureDiv">
            <a href="Reference/Glossary.html#environment-parity">Environment parity</a>
        </div>
    </td>
    <td>
        Builds the same for dotnet.exe, Visual Studio, on build system, or on a dev environments.
        All versioning is done within the csproj build.
    </td>
  </tr>
  <tr>
    <td>
        <div class="featureDiv">
            <a href="https://semver.org/#spec-item-4">Initial development versioning</a>
        </div>
    </td>
    <td>
        Make all <a href="https://semver.org/#spec-item-4">0.x.x versions intial development</a> pre-releases.
    </td>
  </tr>
  <tr>
    <td>
        <div class="featureDiv">
            <a href="Reference/Glossary.html#controlled-host">Controlled/uncontrolled versioning</a>
        </div>
    </td>
    <td>
        Different versioning on controlled and uncontrolled hosts (build system and dev env).
    </td>
  </tr>
  <tr>
    <td>
        <div class="featureDiv">
            <a href="Reference/Glossary.html#build-number">Build numbering</a>
        </div>
    </td>
    <td>
        Detect if building on build system and use build system's build number.
        On a dev environment use a host counter.
    </td>
  </tr>
  <tr>
    <td>
        <div class="featureDiv">
            No limit automatic versioning
        </div>
    </td>
    <td>
        Extended versioning by C# script with a DSL library, and access to host environment and files.
    </td>
  </tr>
  <tr>
    <td>
        <div class="featureDiv">
            Testability
        </div>
    </td>
    <td>
        With environment parity, C# scripting, and built in overrides, test any scenario from a dev environment.
    </td>
  </tr>
</table> 
</div>


## Quick links

* [Getting Started](xref:getting-started)
* Usage
  * [Build Hosts](xref:build-hosts)
  * [C# Script](xref:csharp-script)
* [Reference](xref:concepts)

 
## License

Git2SemVer uses the [MIT license](https://choosealicense.com/licenses/mit/).


## Acknowledgments

This project uses the following tools and libraries. Many thanks to those who created and manage them.

* [Spectre.Console](https://github.com/spectreconsole/spectre.console)
* [Injectio](https://github.com/loresoft/Injectio)
* [JetBrains Annotations](https://www.jetbrains.com/help/resharper/Code_Analysis__Code_Annotations.html)
* [Semver](https://www.nuget.org/packages/Semver) - files copied to create subset
* [NuGet.Versioning](https://www.nuget.org/packages/NuGet.Versioning)
* [NUnit](https://www.nuget.org/packages/NUnit)
* [Moq](https://github.com/devlooped/moq)
* [docfx](https://dotnet.github.io/docfx/)
* <a href="https://www.flaticon.com/free-icons/brain" title="brain icons">Brain icons created by Freepik - Flaticon</a>
