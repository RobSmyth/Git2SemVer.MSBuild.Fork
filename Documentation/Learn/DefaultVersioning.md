---
uid: default-versioning
---
![](../Images/Git2SemVer_banner_840x70.png)

## Default versioning

### Build ID

The [build ID](xref:glossary#build-id) is determined by the [build host](xref:build-hosts) object.
This allows for handline build systems that may not provide a [build number](xref:glossary#build-number) and multiple build number contexts.

Example build ID shemas (using [build host](xref:build-hosts) build number and build context properties):

* <build number>  - Useful when the project's production build system host (e.g: [TeamCity](xref:teamcity)) that provides a [build number](xref:glossary#build-number).
* <build context>.<build number> - Useful for [uncontrolled hosts](xref:uncontrolled-host) where the build context is the host machine's name.
* <build number>.<build context> - Useful when a host provides a psuedo build number ([build ID](xref:glossary#build-id)) and context is required to achive traceability. See [GitHub worfklow](xref:github-workflows) host.

See the [build host](xref:build-hosts) type for details.

## Schema

### Release versioning

| Use                  | Schema                                           |
|:---                  |:---                                              |
| Build system vesions | `1.2.3+<build-id>`                               |
| NuGet - filename     | `1.2.3`                                          |
| NuGet - version      | `1.2.3+<build-id>.<branch>.<short-sha>`          |
| InformationalVerion  | `1.2.3+<build-id>.<branch>.<short-sha>`          |

### Pre-release versioning

| Use                  | Schema                                           |
|:---                  |:---                                              |
| Build system         | `1.2.3-<label>.<build-id>`                       |
| NuGet - filename     | `1.2.3-<label>.<build-id>`                       |
| NuGet - version      | `1.2.3-<label>.<build-id>+<branch>.<short-sha>`  |
| InformationalVerion  | `1.2.3-<label>.<build-id>+<branch>.<short-sha>`  |

### Pre-release labels

| Major | Label             | Controlled   | Branch  | Major  |
| :--   |:--                |:--:          |:-:               |:-:     |
| 0     | ``uncontrolled``  | false        | Any              | >= 0   |
| 0     | ``experimental``  | true         | Any              | = 0    |
| >=1   | GitVerion label   | true         | Non-release      | >= 1   |


## Resulting version examples

### Releases

> [!NOTE]
> Release builds can only be made on the build system.

| Use                  |                                    |
|:---                  |:---                                |
| Build system         | `1.2.3+7658.1`                     |
| NuGet - filename     | `1.2.3`                            |
| NuGet - version      | `1.2.3+7658.1.release-v1.34e6a01`  |
| InformationalVerion  | `1.2.3+7658.1.release-v1.34e6a01`  |

### Prereleases

# [TeamCity builds](#tab/controlled-build-teamcity)

Controlled build using [TeamCity](xref:teamcity).

The versions below assume this is `beta`.

| Use                  | Example version                                          |
|:---                  |:---                                                      |
| Build system         | `1.2.3-beta.7658`                                        |
| NuGet - filename     | `1.2.3-beta.7658`                                        |
| NuGet - version      | `1.2.3-beta.7658+feature-mybranch.6ab397d5`              |
| InformationalVerion  | `1.2.3-beta.7658+feature-mybranch.6ab397d5`              |

# [GitHub builds](#tab/controlled-build-github)

Controlled build using [GitHub worfklow](xref:github-workflows).

The versions below assume this is `beta`.

| Use                  | Example version                                           |
|:---                  |:---                                                       |
| Build system         | `1.2.3-beta.7658.1`                                       |
| NuGet - filename     | `1.2.3-beta.7658.1`                                       |
| NuGet - version      | `1.2.3-beta.7658.1+feature-mybranch.6ab397d5`             |
| InformationalVerion  | `1.2.3-beta.7658.1+feature-mybranch.6ab397d5`             |

# [Uncontrolled builds](#tab/uncontrolled-build)

Uncontrolled build (dev environment). Uses host's managed build number and the machine name as the build context.

The versions below assume a host machine name of `Dev01`.

| Use                  | Example version                                           |
|:---                  |:---                                                       |
| Build system         | not applicable                                            |
| NuGet - filename     | `1.2.3-uncontrolled.212.Dev01`                            |
| NuGet - version      | `1.2.3-uncontrolled.212.Dev01+feature-mybranch.6ab397d5`  |
| InformationalVerion  | `1.2.3-uncontrolled.212.Dev01+feature-mybranch.6ab397d5`  |

---

Depending host selected above. Initial development build version examples:

# [Initial development](#tab/initial-dev/controlled-build-github)

GitHub (a controlled host) build ...

| Use                  | Example version                                           |
|:---                  |:---                                                       |
| Build system         | `0.2.3-experimental.7658.1`                               |
| NuGet - filename     | `0.2.3-experimental.7658.1`                               |
| NuGet - version      | `0.2.3-experimental.7658.1+feature-mybranch.6ab397d5`     |
| InformationalVerion  | `0.2.3-experimental.7658.1+feature-mybranch.6ab397d5`     |

# [Initial development](#tab/initial-dev/controlled-build-teamcity)

TeamCity (a controlled host) build ...

| Use                  | Example version                                           |
|:---                  |:---                                                       |
| Build system         | `0.2.3-experimental.7658`                                 |
| NuGet - filename     | `0.2.3-experimental.7658`                                 |
| NuGet - version      | `0.2.3-experimental.7658+feature-mybranch.6ab397d5`       |
| InformationalVerion  | `0.2.3-experimental.7658+feature-mybranch.6ab397d5`       |

# [Initial development](#tab/initial-dev/uncontrolled-build)

A dev environmemnt (an uncontrolled host) build ...

| Use                  | Example version                                           |
|:---                  |:---                                                       |
| Build system         | not applicable                                            |
| NuGet - filename     | `0.2.3-experimental.212.Dev01`                            |
| NuGet - version      | `0.2.3-experimental.212.Dev01+feature-mybranch.6ab397d5`  |
| InformationalVerion  | `0.2.3-experimental.212.Dev01+feature-mybranch.6ab397d5`  |

---
