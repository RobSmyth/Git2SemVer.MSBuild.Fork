---
uid: logging
---

<div style="background-color:#944248;padding:0px;margin-bottom:0.5em">
  <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/Git2SemVer_banner_840x70.png"/>
</div>

# Logging

## Git2SemVer.MSBuild

On every build [Git2SemVer.MSBuild](xref:git2semver-msbuild) creates a log file `Git2SemVer.MSBuild.log` in the project's intermediate (obj) directory.
This file is overwritten on every build.

## Git2SemVer.Tool

On every command the [Git2SemVer.Tool](xref:git2semver-tool) tool creates a log file `Git2SemVer.Tool.log` in the user's `%appdata%\GitSemVer` folder 
which is typically `C:\Users\<username>\AppData\Local\Git2SemVer`
