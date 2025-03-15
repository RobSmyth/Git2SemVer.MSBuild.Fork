---
uid: versioning
---

<div style="background-color:#944248;padding:0px;margin-bottom:0.5em">
  <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/Git2SemVer_banner_840x70.png"/>
</div>

# Versioning

Git2SemVer generates versioning information and passes this to the optional [C# script](xref:csharp-script) for modification before being passed back to the build.

![](../Images/MSBuild_tasks_01.png)


## Release versioning

Format:

```
  <major>.<minor>.<patch>[+<build-id>.<branch>.<commit-sha>]
```

Where:

* `<build-id>` is the [build ID or build number](xref:build-id).
* `<branch>` is the [branch name](xref:branch-naming).
* `<commit-sha>` is the [git commit SHA](xref:commit-sha).

Examples:

| Use                   | Schema                                           |
|:---                   |:---                                              |
| Build system          | `1.2.3+<build-id>`                               |
| NuGet - filename      | `1.2.3`                                          |
| Informational verion  | `1.2.3+<build-id>.<branch>.<commit-sha>`         |

> [!IMPORTANT]  
> All [initial development](https://semver.org/#spec-item-4) versions (0.x.x) are pre-releases.

## Pre-release versioning

Format:

```
  <major>.<minor>.<patch>-<maturity-label>.<build-id>[+<branch>.<commit-sha>]
```

Where:

* `<maturity-label>` is the [pre-release maturity label](xref:maturity-identifier).
* `<build-id>` is the [build ID or build number](xref:build-id).
* `<branch>` is the [branch name](xref:branch-naming).
* `<commit-sha>` is the [git commit SHA](xref:commit-sha).

Examples:

| Use                   | Schema                                           |
|:---                   |:---                                              |
| Build system          | `1.2.3-<label>.<build-id>`                       |
| NuGet - filename      | `1.2.3-<label>.<build-id>`                       |
| Informational verion  | `1.2.3-<label>.<build-id>+<branch>.<commit-sha>` |


## Initial development versioning (0.x.x)

All versions with a 0 major number (0.x.x) are [initial development](https://semver.org/#spec-item-4) build versions.
Git2SemVer makes all initial development build pre-release build with "InidialDev added to the maturity ID as shown in
the table below.

### [Initial development builds](#tab/initial-dev-builds)

| Branch type      | Maturity Label     |
|:---              |:--                 |
| Release          | `InitialDev`       |
| RC               | `rc-InitialDev`    |
| Feature          | `beta-InitialDev`  |
| Development      | `alpha-InitialDev` |

### [Builds with Major >= 1](#tab/post-initial-dev-builds)

| Branch type      | Maturity Label     |
|:---              |:--                 |
| Release          | not applicable     |
| RC               | `rc`               |
| Feature          | `beta`             |
| Development      | `alpha`            |

---

> [!NOTE]
> Pre-release maturity labels are configurable by setting the MSBuild property [Git2SemVer_BranchMaturityPattern](xref:msbuild-properties).
> Values shown here are defaults.

## Examples

### Releases

#### [TeamCity builds](#tab/controlled-build-teamcity)

| Version               | Example          |
|:---                   |:---                             |
| Build system          | `1.2.3+7658`                    |
| NuGet - filename      | `1.2.3`                         |
| Informational version | `1.2.3+7658.release-v1.34e6a01` |

#### [GitHub builds](#tab/controlled-build-github)

| Version               | Example         |
|:---                   |:---                               |
| Build system          | `1.2.3+7658.1`                    |
| NuGet - filename      | `1.2.3`                           |
| Informational version | `1.2.3+7658.1.release-v1.34e6a01` |

#### [Uncontrolled builds](#tab/uncontrolled-build)

| Version               | Example          |
|:---                   |:---                                       |
| Build system          | `1.2.3+DevPCName.7658`                    |
| NuGet - filename      | `1.2.3`                                   |
| Informational version | `1.2.3+DevPCName.7658.release-v1.34e6a01` |

---

### Prereleases

#### [TeamCity builds](#tab/controlled-build-teamcity)

[TeamCity](xref:teamcity) build versions.

The versions below assume this is `beta`.

| Version               | Example                                |
|:---                   |:---                                         |
| Build system          | `1.2.3-beta.7658`                           |
| NuGet - filename      | `1.2.3-beta.7658`                           |
| Informational version | `1.2.3-beta.7658+feature-mybranch.6ab397d5` |

#### [GitHub builds](#tab/controlled-build-github)

[GitHub worfklow](xref:github-workflows) build versions.

The versions below assume this is `beta`.

| Version | Example                                           |
|:---                   |:---                                           |
| Build system          | `1.2.3-beta.7658.1`                           |
| NuGet - filename      | `1.2.3-beta.7658.1`                           |
| Informational version | `1.2.3-beta.7658.1+feature-mybranch.6ab397d5` |

#### [Uncontrolled builds](#tab/uncontrolled-build)

[Uncontrolled (dev environment)](xref:uncontrolled-host) builds. Uses host's managed build number and the machine name as the build context.

The versions below assume a host machine name of `DevPCName`.

| Version | Example                                           |
|:---                  |:---                                                       |
| Build system         | `1.2.3-beta.DevPCName.212`                            |
| NuGet - filename     | `1.2.3-beta.DevPCName.212`                            |
| Informational version | `1.2.3-beta.DevPCName.212+feature-mybranch.6ab397d5`  |

---

