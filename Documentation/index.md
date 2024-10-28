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

![](Images/Git2SemVer_banner_840x70.png)

[![Current Version](https://img.shields.io/nuget/v/NoeticTools.Git2SemVer.MSBuild?label=Git2SemVer.Msbuild)](https://www.nuget.org/packages/NoeticTools.Git2SemVer.MSBuild)
[![Current Version](https://img.shields.io/nuget/v/NoeticTools.Git2SemVer.Tool?label=Git2SemVer.Tool)](https://www.nuget.org/packages/NoeticTools.Git2SemVer.Tool)
[![Release Build](https://github.com/NoeticTools/Git2SemVer/actions/workflows/dotnet.yml/badge.svg)](https://github.com/NoeticTools/Git2SemVer/actions/workflows/dotnet.yml)
<a href="https://github.com/NoeticTools/Git2SemVer">
  ![Static Badge](https://img.shields.io/badge/GitHub%20project-944248?logo=github)
</a>

# Git2SemVer

<div style="margin-left:0px; margin-top:-5px; margin-bottom:35px; font-family:Calibri; font-size:1.3em;">
No limits .NET solution versioning.</div>

> [!NOTE]  
> This project is in early development and may undergo large changes
> before the first stable release 1.0.0. 
>
> Early testing and feedback would be great!

Git2SemVer is a <a href="https://semver.org">Semantic Versioning</a> framework for .NET solution and project versioning.
It works the same in both dotnet CLI and Visual Studio builds and for both developers and build system builds.
It just happens using <a href="https://www.conventionalcommits.org/en/v1.0.0/">Conventional Commits</a> message elements and Git tagging of each release.

For solution versioning (were all projects in a .NET solution to share a common versioning) use the dotnet CLI tool `Git2SemVer.Tool` is used to setup the solution.
For project versioning, adding the nuget package `Git2SemVer.MSBuild` to the .NET project.
With the versioning being done within MSBuild, no build system version generation steps are needed and it works the same for both dotnet CLI and Visual Studio.

For no limits customisation, Git2SemVer has an in-built C# script API that can change any part of the versioning.

# Features

<div style="margin:5px; text-align:center; width:95%">
<table>

 <tr>
    <td style="width:33%">
      <img src="https://noetictools.github.io/Git2SemVer/Images/OpenSource_128x128.png" height=128 />
    </td>
    <td style="width:33%">
      <a href="https://semver.org/">
        <img src="https://noetictools.github.io/Git2SemVer/Images/SemVer213x128(dark).png" height=128 />
      </a>
    </td>
    <td  style="width:33%">
      <a href="https://www.conventionalcommits.org/en/v1.0.0/">
        <img src="https://noetictools.github.io/Git2SemVer/Images/ConventionalCommits_128x128.png" height=128 />
      </a>
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
        <p>Free and open source.</p>
    </div>
  </td>

  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody">
      <p style="text-align:center">Benefit from comprehensive Semmantic Version compliance with:
      <a href="https://noetictools.github.io/Git2SemVer/Reference/Glossary.html##initial-development">Initial development</a> versioning, 
      <a href="https://semver.org/#spec-item-5">Release versioning.</a>, 
      and automatic <a href="https://noetictools.github.io/Git2SemVer/Reference/Glossary.html##build-number">build numbering</a>.</p>
    </div>
  </td>

  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody">
        <p>Use Conventional Commits commit message elements to bump versions:</p>
        <p style="margin-top: 0em;margin-bottom: 0em;">
          <i>fix:</i> | <i>feat:</i> | <i>BREAKING CHANGE:</i>
        </p>
    </div>
  </td>
</tr>

<!-- Row 2 -->

<tr>
    <td >
      <a href="https://noetictools.github.io/Git2SemVer/Reference/Glossary.html##environment-parity">
        <img src="https://noetictools.github.io/Git2SemVer/Images/consistency_128x128.png" height=128 />
      </a>
    </td>
    <td >
      <p style="font-size:100px; margin:0px;color:DarkCyan;">#</p>
    </td>
    <td >
      <img src="https://noetictools.github.io/Git2SemVer/Images/CSharp_128x128.png" height=128 />
    </td>
</tr>
<tr>
  <td>
    <div class="featureTitle">
        <a href="https://noetictools.github.io/Git2SemVer/Reference/Glossary.html##environment-parity">Environment Parity</a>
    </div>
  </td>
  <td>
    <div class="featureTitle">
      <a href="https://noetictools.github.io/Git2SemVer/Reference/Glossary.html##build-number">
        Build Numbering
      </a>
    </div>
  </td>
  <td>
    <div class="featureTitle">
        <a href="/Usage/CSharpScripting/CSharpScript.html">C# Scripting</a>
    </div>
  </td>
</tr>
<tr>
  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody" style="vertical-align:top">
      <p>
      Visual Studio, VS Code, and dotnet CLI are all the same to Git2SemVer.
      Build system and developer environments all get versioning too.
      </p>
      <p>
      No custom versioning build steps or scripts required.
      </p>
    </div>
  </td>
  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody">
      <p>
        Automatic build numbering on all builds on <b>developer boxes</b> and the build system.
      <p>
      <p>
        Full traceability. <a href="https://noetictools.github.io/Git2SemVer/Reference/Glossary.html##build-height">Build height</a> (commit counting) <b>NOT</b> used.
      </p>
    </div>
  </td>
  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody">
        <p><b>No limits</b> customisable by built-in C# scripting and versioning API.</p>
    </div>
  </td>
</tr>

<!-- Row 3 -->

<tr>
    <td >
      <img src="https://noetictools.github.io/Git2SemVer/Images/VisualStudio_128x128.png" height=128 />
    </td>
    <td >
      <img src="https://noetictools.github.io/Git2SemVer/Images/ComputerMonitor.png" height=128 />
    </td>
    <td >
      <img src="https://noetictools.github.io/Git2SemVer/Images/git_workflow_128x128.png" height=128 />
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
      <p>Implemented in MSBuild it works the same with dotnet Visual Studio and CLI.</p>
      <p>.NET developer friendly.</p>
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
      <a href="/Usage/BuildHosts/TeamCity.html">
        <img src="https://noetictools.github.io/Git2SemVer/Images/TeamCity_128x128.png" height=128 />
      </a>
    </td>
    <td >
      <a href="/Usage/BuildHosts/GitHubWorkflows.html">
        <img src="https://noetictools.github.io/Git2SemVer/Images/github_gray_128x128.png" height=128 />
      </a>
    </td>
    <td >
      <!-- Empty -->
    </td>
</tr>
<tr>
  <td>
    <div class="featureTitle">
      <a href="/Usage/BuildHosts/TeamCity.html">
        TeamCity Integration
      </a>
    </div>
  </td>
  <td>
    <div class="featureTitle">
      <a href="/Usage/BuildHosts/GitHubWorkflows.html">
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
      <img src="https://noetictools.github.io/Git2SemVer/Images/TeamCity-01.png">
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
