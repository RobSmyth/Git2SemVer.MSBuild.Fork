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

![Current Version](https://img.shields.io/nuget/v/NoeticTools.Git2SemVer.MSBuild?label=Git2SemVer.Msbuild)
![Current Version](https://img.shields.io/nuget/v/NoeticTools.Git2SemVer.Tool?label=Git2SemVer.Tool)
[![Release Build](https://github.com/NoeticTools/Git2SemVer/actions/workflows/dotnet.yml/badge.svg)](https://github.com/NoeticTools/Git2SemVer/actions/workflows/dotnet.yml)


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

Git2SemVer includes a dotnet CLI tool (Git2SemVer.Tool) for simple solution versioning setup. 
The setup configures add the Git2SemVer.MSBuild NuGet package to all solution projects.
With the versioning being done within MSBuild, no build system version generation steps are needed and it works the same for both dotnet CLI and Visual Studio.

No complex configuration of branching strategy (like GitFlow or GitHubFlow) and completely customisable using an optional project C# script that can change any or all of the generated version information
Git2SemVer determines Semantic Version from Git commit history and the host environment.

<br/>

<div style="margin:5px; text-align:center; width:95%">
<table>

 <tr>
    <td style="width:33%">
      <img src="Images/OpenSource_128x128.png" height=128 />
    </td>
    <td style="width:33%">
      <img src="Images/SemVer213x128(dark).png" height=128 />
    </td>
    <td  style="width:33%">
      <img src="Images/ConventionalCommits_128x128.png" height=128 />
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
      <p style="text-align:center">Benefit from comprehensive Semmantic Version compliance.</p>
    </div>
    <div class="featureBody">
         <p>Supports <a href="https:Reference/Glossary.html#initial-development">initial development</a> versioning.</p>
         <p><a href="https://semver.org/#spec-item-5">Release versioning.</a></p>
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
      <img src="Images/consistency_128x128.png" height=128 />
    </td>
    <td >
      <p style="font-size:100px; margin:0px;color:DarkCyan;">#</p>
    </td>
    <td >
      <img src="Images/CSharp_128x128.png" height=128 />
    </td>
</tr>
<tr>
  <td>
    <div class="featureTitle">
        <a href="Reference/Glossary.html#environment-parity">Environment Parity</a>
    </div>
  </td>
  <td>
    <div class="featureTitle">
        Build Numbering
    </div>
  </td>
  <td>
    <div class="featureTitle">
        C# Scripting
    </div>
  </td>
</tr>
<tr>
  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody" style="vertical-align:top">
      <p>VS Code, dotnet CLI, Visual Studio are all the same.
        All versioning is done within MSBuild (csproj file).</p>
      <p>Build system and developer environments are all the same.
      No custom versioning build steps or scripts required.</p>
    </div>
  </td>
  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody">
        <p>Automatic <a href="Reference/Glossary.html#build-number">build numbering</a> on all builds on <b>developer boxes</b> and the build system.<p>
        <p>Full traceability. <a href="Reference/Glossary.html#build-height">Build height</a> (commit counting) <b>NOT</b> used.</p>
    </div>
  </td>
  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody">
        <p>Built in C# scripting (csx) can be used to modify all or some of the generated versioning.</p>
        <p>Unlimited customisation with API. A few lines of code makes customisation easy.</p>
    </div>
  </td>
</tr>

<!-- Row 3 -->

<tr>
    <td >
      <img src="Images/VisualStudio_128x128.png" height=128 />
    </td>
    <td >
      <img src="Images/ComputerMonitor.png" height=128 />
    </td>
    <td >
      <img src="Images/git_workflow_128x128.png" height=128 />
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
        Build Host Aware
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
      <p>Implemented in MSBuild it works the same with dotnet CLI and Visual Studio.</p>
      <p>.NET developer friendly.</p>
    </div>
  </td>
  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody">
        <p>The versioning adapts according to the build host.</p>
        <p>Developer box builds get prelease/metadata identifiers make it apparent that is a dev box build.</p>
        <p>See: <a href="Reference/Glossary.html#controlled-host">Controlled/uncontrolled versioning</a></p>
    </div>
  </td>
  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody">
        <p>No Git workflow configuration required. It works the same for GitFlow and GitHub Flow.</p>
    </div>
  </td>
</tr>

<!-- Row 4 -->

<tr>
    <td >
      <img src="Images/TeamCity_128x128.png" height=128 />
    </td>
    <td >
      <img src="Images/GitHub_gray_128x128.png" height=128 />
    </td>
    <td >
      <!-- Empty -->
    </td>
</tr>
<tr>
  <td>
    <div class="featureTitle">
        TeamCity Integration
    </div>
  </td>
  <td>
    <div class="featureTitle">
        GitHub Actions
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
      <p>Automatic build system detection with server build number (label) updating.</p>
      <img src="Images/TeamCity-01.png">
    </div>
  </td>
  <td class="featureBody" style="vertical-align:top">
    <div class="featureBody">
        <p>Built in support for builds using GitHub Actions.</p>
        <p>Constructs build number from run and run attempt numbers.</p>
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
* [Semver](https://www.nuget.org/packages/Semver) - files copied to create subset
* [NuGet.Versioning](https://www.nuget.org/packages/NuGet.Versioning)
* [NUnit](https://www.nuget.org/packages/NUnit)
* [Moq](https://github.com/devlooped/moq)
* [docfx](https://dotnet.github.io/docfx/)
* <a href="https://www.flaticon.com/free-icons/brain" title="brain icons">Brain icons created by Freepik - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/consistent" title="consistent icons">Consistent icons created by Freepik - Flaticon</a>
