---
uid: branch-naming
---

<div style="background-color:#944248;padding:0px;margin-bottom:0.5em">
  <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/Git2SemVer_banner_840x70.png"/>
</div>

[![Current Version](https://img.shields.io/nuget/v/NoeticTools.Git2SemVer.MSBuild?label=Git2SemVer.MSBuild)](https://www.nuget.org/packages/NoeticTools.Git2SemVer.MsBuild)
<a href="https://github.com/NoeticTools/Git2SemVer">
  ![Static Badge](https://img.shields.io/badge/GitHub%20project-944248?logo=github)
</a>


# Branch naming

The branch name determines if the build will have release or pre-release versioning.
If a pre-release then the branch name is also used to determine the build maturity identifier (e.g: `alpha` or `beta`). 
See [Build maturity identifier](xref:maturity-identifier).

> [!NOTE]
> If a branch name is used in version metadata, [invalid Semmmantic Versioning characters](https://semver.org/#spec-item-10) are replaced with the "-" characters.
> This is to ensure Semmmantic Versioning compliance and compatibility with common tools.
