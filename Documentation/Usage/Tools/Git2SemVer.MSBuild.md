---
uid: git2semver-msbuild
---

<div style="background-color:#944248;padding:0px;margin-bottom:0.5em">
  <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/Git2SemVer_banner_840x70.png"/>
</div>

# Git2SemVer.MSBuild

[![`Current Version`](https://img.shields.io/nuget/v/NoeticTools.Git2SemVer.MSBuild?label=Git2SemVer.Msbuild)](https://www.nuget.org/packages/NoeticTools.Git2SemVer.MSBuild)

Git2SemVer.MSBuild is a nuget package that adds MSBuild tasks to .NET C# projects to version the project on every build.

[Git2SemVer.Tool](xref:git2semver-tool) configures all projects in a solution to use `Git2SemVer.MSBuild` and configures version sharing between the projects.

## Installing

First check [prerequisites](xref:prerequisites) and then:

* For project versioning - Add the nuget package [NoeticTools.Git2SemVer.MSBuild](https://www.nuget.org/packages/NoeticTools.Git2SemVer.MSBuild) to a project. 

* For solution versioning - Use [Git2SemVer.Tool](xref:git2semver-tool). It will configure projects to use the NoeticTools.Git2SemVer.MSBuild package.

