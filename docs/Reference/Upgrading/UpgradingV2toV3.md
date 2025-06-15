---
uid: Upgrading_v1_to_v2
---

<div style="background-color:#944248;padding:0px;margin-bottom:0.5em">
  <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/Git2SemVer_banner_840x70.png"/>
</div>

[![Current Version](https://img.shields.io/nuget/v/NoeticTools.Git2SemVer.MSBuild?label=Git2SemVer.MSBuild)](https://www.nuget.org/packages/NoeticTools.Git2SemVer.MsBuild)
<a href="https://github.com/NoeticTools/Git2SemVer">
  ![Static Badge](https://img.shields.io/badge/GitHub%20project-944248?logo=github)
</a>


# Upgrading from Git2SemVer.MSBuild V2 to V3

This page lists breaking changes in Git2SemVer.MSBuild 3.0.0.

## TeamCity - build time statistic

Git2SemVer.MSBuild 3.0.0 has a breaking change that will impact users that are using versioning build time statistic on TeamCity ([#43](https://github.com/NoeticTools/Git2SemVer.Core/issues/43)).
The TeamCity versioning build time statistic was renamed from to `Git2SemVerRunTime_sec` to `git2semver.runtime.seconds`.
If using a TeamCity build system and this build time statistic then the name will need to be manually changed in TeamCity.

## Linux - libdl.so

Builds on Linux hosts (e.g: Ubuntu >= 22) may break with an error that the file `libdl.so` was not found.
Git2SemVer.MSBuild 3.0.0 uses LibGit2Sharp to access Git history. LibGit2Sharp requires the `libdl.so` library file.
Some manual configuration may be required on the build host to make this library avaiable as `libdl.so`.

For example, if a different version like `libdl.so.2`is available the a symlink to `libdl.so` may resolve the issue:

```
whereis libdl.so.2
libdl.so.2: /usr/lib/libdl.so.2 /usr/lib32/libdl.so.2
$ sudo ln -s /usr/lib/libdl.so.2 /usr/lib/libdl.so
$ sudo ln -s /usr/lib32/libdl.so.2 /usr/lib32/libdl.so
```
