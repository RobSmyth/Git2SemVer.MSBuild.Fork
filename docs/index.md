---
_layout: landing
---
<style>

.featureTitle {
  font-size:1.2em;
}

.featureBody {
  font-size:1.0em;
}

.featureBodyLeftAlign {
  font-size:1.0em;
  text-align:left;
}

table, tr {
  border:none !important;
}

td {
  border:none !important;
  width:300px;
}

a 
{
  text-decoration: none; 
}
</style>

<div style="background-color:#944248;padding:0px;margin-bottom:0.5em">
  <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/Git2SemVer_banner_840x70.png"/>
</div>

[![Current Version](https://img.shields.io/nuget/v/NoeticTools.Git2SemVer.MSBuild?label=Git2SemVer.Msbuild)](https://www.nuget.org/packages/NoeticTools.Git2SemVer.MSBuild)
[![Current Version](https://img.shields.io/nuget/v/NoeticTools.Git2SemVer.Tool?label=Git2SemVer.Tool)](https://www.nuget.org/packages/NoeticTools.Git2SemVer.Tool)
[![Release Build](https://github.com/NoeticTools/Git2SemVer/actions/workflows/dotnet.yml/badge.svg)](https://github.com/NoeticTools/Git2SemVer/actions/workflows/dotnet.yml)
<a href="https://github.com/NoeticTools/Git2SemVer">
  ![Static Badge](https://img.shields.io/badge/GitHub%20project-944248?logo=github)
</a>

# Git2SemVer

<div style="margin-left:0px; margin-top:-5px; margin-bottom:35px; font-family:Calibri; font-size:1.3em;">
No limits .NET solution versioning.</div>

Git2SemVer is a Visual Studio and developer friendly <a href="https://semver.org">Semantic Versioning</a> framework for .NET solution and project versioning.
It works the same with both Visual Studio and dotnet CLI builds. 
Every build, on both developer boxes and the build system, get traceable build numbering (no commit counting).

This tool is best for suited teams that:

* Wants true <a href="https://semver.org">Semantic Versioning</a>.
* Uses <a href="https://www.conventionalcommits.org/en/v1.0.0/">Conventional Commits</a> to automatically generate change logs.
* Uses branches to separate releasable code from feature or under development code (e.g: GitHub flow or GitFlow). 
* Only releases builds from a build system (or controlled host).
* Wants to avoid custom build scripts, or tools, on a build system.
* Uses Visual Studio as well as dotnet CLI.
* Values full traceability for every build regardless if on a build system or an uncontrolled developer box (commit counts/depth will not do).
* Needs unique versioning customisation that internal C# scripting may provide.


# Features

<div style="margin:5px; text-align:center; width:95%">
<table>

 <tr>
     <td style="width:33%">
      <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/VisualStudio_128x128.png" height=64 />
    </td>
    <td style="width:33%">
      <a href="https://semver.org/">
        <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/SemVer213x128(dark).png" height=64 />
      </a>
    </td>
    <td  style="width:33%">
      <a href="https://www.conventionalcommits.org/en/v1.0.0/">
        <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/ConventionalCommits_128x128.png" height=64 />
      </a>
    </td>
</tr>

<tr>
  <td>
    <div class="featureTitle">
        Visual Studio Friendly
    </div>
  </td>
  <td>
    <div class="featureTitle">
      <a href="https://semver.org/">Semmantic Versioning</a>
    </div>
  </td>
  <td>
    <div class="featureTitle" >
        <a href="https://www.conventionalcommits.org/en/v1.0.0/">Conventional Commits</a>
    </div>
  </td>
</tr>

<tr>
  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody">
      <p>Versioning, with build numbering, on every Visual Studio or dotnet CLI build.</p>
      <p>.NET developer friendly. Fast and silent</p>
    </div>
  </td>

  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody">
      <p style="text-align:center">
        Benefit from comprehensive industry standard Semmantic Version compliance.
      </p>
      <p>
        Only tool compliant with <a href="https://noetictools.github.io/Git2SemVer.MSBuild/Reference/Glossary.html##initial-development">initial development versioning.</a> 
    </div>
  </td>

  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody">
        <p style="margin-top: 0em;margin-bottom: 0em;">
          Use the same commit message standards for versioning that you also use for automated changelog generation.
        </p>
    </div>
  </td>
</tr>

<!-- Row 2 -->

<tr>
    <td >
      <a href="https://noetictools.github.io/Git2SemVer.MSBuild/Reference/Glossary.html##environment-parity">
        <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/consistency_128x128.png" height=64 />
      </a>
    </td>
    <td >
      <p style="font-size:50px; margin:0px;color:DarkCyan;">#</p>
    </td>
    <td >
      <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/CSharp_128x128.png" height=64 />
    </td>
</tr>
<tr>
  <td>
    <div class="featureTitle">
        <a href="https://noetictools.github.io/Git2SemVer.MSBuild/Reference/Glossary.html##environment-parity">Environment Parity</a>
    </div>
  </td>
  <td>
    <div class="featureTitle">
      <a href="https://noetictools.github.io/Git2SemVer.MSBuild/Reference/Glossary.html##build-number">
        Build Numbering
      </a>
    </div>
  </td>
  <td>
    <div class="featureTitle">
        <a href="https://noetictools.github.io/Git2SemVer.MSBuild/Usage/CSharpScripting/CSharpScript.html">C# Scripting</a>
    </div>
  </td>
</tr>
<tr>
  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody" style="vertical-align:top">
      <p>
        Visual Studio, VS Code, and dotnet CLI are all the same to Git2SemVer.
        Build system and developer environments all get versioning without custom build steps or scripts.
      </p>
    </div>
  </td>
  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody">
      <p>
        Automatic build numbering on all <b>developer boxes</b> and the build system builds.
      <p>
      <p>
        Full traceability.
        <a href="https://noetictools.github.io/Git2SemVer.MSBuild/Reference/Glossary.html##build-height">Build height</a> <b>NOT</b> used.
      </p>
    </div>
  </td>
  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody">
        <p>
          <b>No limits customisable</b> by built-in C# scripting with a versioning API.
        </p>
    </div>
  </td>
</tr>

<!-- Row 3 -->

<tr>
    <td style="width:33%">
      <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/OpenSource_128x128.png" height=64 />
    </td>
    <td >
      <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/ComputerMonitor.png" height=64 />
    </td>
    <td >
      <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/git_workflow_128x128.png" height=64 />
    </td>
</tr>
<tr>
  <td>
    <div class="featureTitle" >
        Open Source
    </div>
  </td>
  <td>
    <div class="featureTitle">
        Build Host Adaptive Versioning
    </div>
  </td>
  <td>
    <div class="featureTitle">
        Git Workflow Agnostic
    </div>
  </td>
</tr>
<tr>
  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody">
        <p>Free and open source.</p>
    </div>
  </td>
  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody">
        <p>The versioning adapts according to the build host.</p>
        <p>e.g: Prelease/metadata identifiers are added to identify dev box builds.</p>
    </div>
  </td>
  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody">
        <p>
        No Git workflow configuration required. It works the same for GitFlow and GitHub Flow.
        </p>
    </div>
  </td>
</tr>

<!-- Row 4 -->

<tr>
    <td >
      <a href="https://noetictools.github.io/Git2SemVer.MSBuild/Usage/BuildHosts/TeamCity.html">
        <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/TeamCity_128x128.png" height=64 />
      </a>
    </td>
    <td >
      <a href="https://noetictools.github.io/Git2SemVer.MSBuild/Usage/BuildHosts/GitHubWorkflows.html">
        <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/github_gray_128x128.png" height=64 />
      </a>
    </td>
    <td >
      <!-- Empty -->
    </td>
</tr>
<tr>
  <td>
    <div class="featureTitle">
      <a href="https://noetictools.github.io/Git2SemVer.MSBuild/Usage/BuildHosts/TeamCity.html">
        TeamCity Integration
      </a>
    </div>
  </td>
  <td>
    <div class="featureTitle">
      <a href="https://noetictools.github.io/Git2SemVer.MSBuild/Usage/BuildHosts/GitHubWorkflows.html">
        GitHub Workflows
      </a>
    </div>
  </td>
  <td>
    <div class="featureTitle">
      <!-- Empty -->
    </div>
  </td>
</tr>
<tr>
  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody">
      <p>
        Automatic build system detection with server build number (label) updated with
        a build version specifically adapted for TeamCity.
      </p>
      <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/TeamCity-01.png">
    </div>
  </td>
  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody">
        <p>Built in support for builds using GitHub Actions.</p>
        <p>Constructs build number from run and run attempt numbers
        and adapts the version for GitHub builds.</p>
    </div>
  </td>
  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody">
      <!-- Empty -->
    </div>
  </td>
</tr>
</table> 
</div>

<br/>

## Quick introduction

You identify a release by adding a git tag like "`v1.2.3`" to the release's commit.
Then, Git2SemVer works out build version of following commits by identifying breaking changes, new features, or bug fixes from from your <a href="https://www.conventionalcommits.org/en/v1.0.0/">Conventional Commits</a> 
compliant commit messages. You already use Conventional Commits generate your changelog so it is getting two for the price of one.

Versioning includes:

* .NET file and assembly versions
* NuGet package version (including version in filename)

The branch name determines if the build is a release build or a `alpha`/`beta`/`rc` pre-release build.
See [Build maturity identifier](xref:maturity-identifier) for more information.

For no limits customisation, Git2SemVer detects and executes an optional [C# script](xref:csharp-script) that can change any part of the versioning.

It can be configured for any mix of solution versioning and individual project versioning without external build-time tools.
No build system version generation steps are needed, keeps developer and build environments simple and aligned.

## Quick links

* [Getting Started](xref:getting-started)
* [Default Versioning](xref:versioning)
* Usage
  * [Build Hosts](xref:build-hosts)
  * [C# Script](xref:csharp-script)

 
## License

Git2SemVer uses the [MIT license](https://choosealicense.com/licenses/mit/).


## Acknowledgments

This project uses the following tools and libraries. Many thanks to those who created and manage them.

* [Spectre.Console](https://github.com/spectreconsole/spectre.console)
* [Injectio](https://github.com/loresoft/Injectio)
* [JetBrains Annotations](https://www.jetbrains.com/help/resharper/Code_Analysis__Code_Annotations.html)
* [TeamCity.ServiceMessages](https://github.com/JetBrains/TeamCity.ServiceMessages)
* [Semver](https://www.nuget.org/packages/Semver) - files copied to create subset
* [NuGet.Versioning](https://www.nuget.org/packages/NuGet.Versioning)
* [NUnit](https://www.nuget.org/packages/NUnit)
* [Moq](https://github.com/devlooped/moq)
* [docfx](https://dotnet.github.io/docfx/)
* [JsonPeek](https://www.clarius.org/json/)
* <a href="https://www.flaticon.com/free-icons/brain" title="brain icons">Brain icons created by Freepik - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/consistent" title="consistent icons">Consistent icons created by Freepik - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/programmer" title="programmer icons">Programmer icons created by Flowicon - Flaticon</a>
