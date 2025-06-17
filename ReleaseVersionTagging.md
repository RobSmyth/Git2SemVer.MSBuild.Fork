---
uid: git2semver-release-version-tag
---

<div style="background-color:#944248;padding:0px;margin-bottom:0.5em">
  <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/Git2SemVer_banner_840x70.png"/>
</div>

[![Current Version](https://img.shields.io/nuget/v/NoeticTools.Git2SemVer.Tool?label=Git2SemVer.Tool)](https://www.nuget.org/packages/NoeticTools.Git2SemVer.Tool)
<a href="https://github.com/NoeticTools/Git2SemVer">
  ![Static Badge](https://img.shields.io/badge/GitHub%20project-944248?logo=github)
</a>

# Release version tagging

When a build is released the build's Git commit is tagged with a release version tag. This tag use the format:

```winbatch
  v<major>.<minor>.<patch>
```

For example:

```winbatch
  v1.2.3
```

> [!NOTE]
> The intent is to tag the commit for a build that is being released.
> Semantic versioning rules have determined what the next release will be and the version tag marks the commit as that release.
> The tag's version should be the same as the version seen in the build that is being released.
>
> Normally the release build is made and tested before the commit is tagged.

Rebuilds of the commit with a release version tag will 
produce builds with the same released base version but with different build numbers in the full version metadata.

The commit being tagged does not need to be the most recent commit on a release branch. 
If it is not the most recent then rebuilding following commits will result in their versions following the released version according to semantic versioning rules.

