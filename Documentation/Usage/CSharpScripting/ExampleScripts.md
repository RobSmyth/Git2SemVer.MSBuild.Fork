---
uid: example-scripts
---

<div style="background-color:#944248;padding:0px;margin-bottom:0.5em">
  <img src="https://noetictools.github.io/Git2SemVer/Images/Git2SemVer_banner_840x70.png"/>
</div>

# Example C# Script

Example script with schema and example versions.

## Script

> [!NOTE]
> When building on a GitHub host (using GitHub actions) BuildContext is `<github.run_attempt>`. This number starts as "1".
> 
> When building on a build system that provides a build number, BuildContext is "0". 

Two versions of the scripts are given.
On for building on GitHub, GitHub actions, and the other for a build system that provides a build number such as TeamCity.
If the project does not build on GitHub the TeamCity version is recommended as it produces shorter and simpler build system labels.

The TeamCity version of the script makes the build system label shorter by removing ".0" build context suffix.
It also simplifies the release information metadata by removing the "0" build context.

Highlighted lines are lines that are different between the two scrips.

**TODO**


# [GitHub](#tab/Github)

[!code-csharp[](GitHubScript.csx?highlight=17,32)]

# [TeamCity](#tab/TeamCity)

[!code-csharp[](TeamCityScript.csx?highlight=17,32)]

---

## Schema

### Release versioning

| Use                  | Schema                                                        |
|:---                  |:---                                                           |
| Build system vesions | `1.2.3+<build>[.<build-context>]`                             |
| Package version      | `1.2.3`                                                       |
| InformationalVerion  | `1.2.3+<build>[.<build-context>].<branch>.<short-sha>`        |

### Pre-release versioning

| Use                  | Schema                                                        |
|:---                  |:---                                                           |
| Build system         | `1.2.3-<label>.<build>[.<build-context>]`                     |
| Package version      | `1.2.3-<label>.<build>.<build-context>`                       |
| InformationalVerion  | `1.2.3-<label>.<build>.<build-context>+<branch>.<short-sha>`  |

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
| Package version      | `1.2.3`                            |
| InformationalVerion  | `1.2.3+7658.1.release-v1.34e6a01`  |

### Prereleases

# [TeamCity builds](#tab/controlled-build-teamcity)

Controlled build using TeamCity.

The versions below assume this is `beta`.

| Use                  | Example version                                          |
|:---                  |:---                                                      |
| Build system         | `1.2.3-beta.7658`                                        |
| Package version      | `1.2.3-beta.7658`                                        |
| InformationalVerion  | `1.2.3-beta.7658+feature-mybranch.6ab397d5`              |

# [GitHub builds](#tab/controlled-build-github)

Controlled build using GitHub actions.

The versions below assume this is `beta`.

| Use                  | Example version                                           |
|:---                  |:---                                                       |
| Build system         | `1.2.3-beta.7658.1`                                       |
| Package version      | `1.2.3-beta.7658.1`                                       |
| InformationalVerion  | `1.2.3-beta.7658.1+feature-mybranch.6ab397d5`             |

# [Uncontrolled builds](#tab/uncontrolled-build)

Uncontrolled build (dev environment). Uses host's Git2SemVer managed build number and the machine name as the build context.

The versions below assume a host machine name of `Dev01`.

| Use                  | Example version                                           |
|:---                  |:---                                                       |
| Build system         | not applicable                                            |
| Package version      | `1.2.3-uncontrolled.212.Dev01`                            |
| InformationalVerion  | `1.2.3-uncontrolled.212.Dev01+feature-mybranch.6ab397d5`  |

---

Depending host selected above. Initial development build version examples:

# [Initial development](#tab/initial-dev/controlled-build-github)

GitHub (a controlled host) build ...

| Use                  | Example version                                           |
|:---                  |:---                                                       |
| Build system         | `0.2.3-experimental.7658.1`                               |
| Package version      | `0.2.3-experimental.7658.1`                               |
| InformationalVerion  | `0.2.3-experimental.7658.1+feature-mybranch.6ab397d5`     |

# [Initial development](#tab/initial-dev/controlled-build-teamcity)

TeamCity (a controlled host) build ...

| Use                  | Example version                                           |
|:---                  |:---                                                       |
| Build system         | `0.2.3-experimental.7658`                                 |
| Package version      | `0.2.3-experimental.7658`                                 |
| InformationalVerion  | `0.2.3-experimental.7658+feature-mybranch.6ab397d5`       |

# [Initial development](#tab/initial-dev/uncontrolled-build)

A dev environmemnt (an uncontrolled host) build ...

| Use                  | Example version                                           |
|:---                  |:---                                                       |
| Build system         | not applicable                                            |
| Package version      | `0.2.3-experimental.212.Dev01`                            |
| InformationalVerion  | `0.2.3-experimental.212.Dev01+feature-mybranch.6ab397d5`  |

---
