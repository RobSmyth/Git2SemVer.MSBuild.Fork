---
uid: Upgrading_v1_to_v2
---

<div style="background-color:#944248;padding:0px;margin-bottom:0.5em">
  <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/Git2SemVer_banner_840x70.png"/>
</div>
<br/>

# Upgrading from Git2SemVer.MSBuild V2 to V3

Git2SemVer.MSBuild 3.0.0 has breaking changes that will impact users that are either:
* Using versioning build time statistic on TeamCity ([#43](https://github.com/NoeticTools/Git2SemVer.Core/issues/43)).
* Using custom C# script (csx) code. Some changes to API.

The TeamCity versioning build time statistic was renamed from to `Git2SemVerRunTime_sec` to `git2semver.runtime.seconds`.
If using a TeamCity build system and this build time statistic then the name will need to be manually changed in TeamCity.

The `MsBuildGlobalProperties` property is no longer available to C# script (csx) code.
