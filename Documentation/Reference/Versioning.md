---
uid: versioning
---
![](../Images/Git2SemVer_banner_840x70.png)

# Versioning Schemes


### Terminology: Versions, releases, builds, and product names

> [!NOTE]
> Versioning here refers to software artifact versioning which is important to a software team.
> Product versioning, which is important to a product owner, is a different concern.
> Depending on the team's development methodology and who the team delivers to, these may be very different versions.

A version can be a release version, build version, or a combination of both (e.g: Semantic Versioning).
It is normal for an artificat to have multiple types of versioning.

For example:

* A NuGet package's name include a release version while the information version, found in the package, can provide additional build versioning information.
* A .NET assembly has an assembly version and an information version.

* 
#### Release versioning

A `release version` is a version identifies a release. It is bumped when a [release](#releases) is made.
The project must define what a release is but is typically when a tested build is released to the customer or end users.


#### Build versioning

A `build version` is a version that identifies a build. It includes a [build number](#build-number) that is bumped on every build.

A build version provides traceability to the build and therefore source code.


#### Releases

What is a "release" depends on context.

From the point of view of software artifact versioning, a release is when when a team releases tested artifactions to a customer.
In a [CI](https://en.wikipedia.org/wiki/Continuous_integration) environment a release build is typically a release candidate that may become the release.
In a [CD](https://en.wikipedia.org/wiki/Continuous_delivery) environment every (or almost every) release build is a release.

Semantic Versioning, for example, calls for certain version bumping after a release. So understanding what a release is is important.

To define what a release is the software team needs to identify who is the customer of its artifacts.
This is commonly another team within an organisation like a product integration test team or product marketing that may include the artifacts in a product release.

A product releases are triggered by:

* Software achieving a [MVP](https://en.wikipedia.org/wiki/Minimum_viable_product)
* When other components, that may not be software, are also available
* Separate V&V cycles
* Unforseen circumstances such as an urgent feature for new customer or an urgent bug fix
* Commercial needs


### Controlled/uncontrolled

A controlled build is a build performed on the project's build system.
Uncontrolled builds are usually "developer box" builds and usually a low [maturity](build-maturity) and are traceable as:

* It is not possible to know what was built. For example: The code may not, ever, be commited or the commit later deleted/squashed.
* The tools and libraries used to build are uncontrolled. Hence the build is not reproducable.
* It is not reasonably possible to have a _unique_ build number. e.g: The number of commits since X may change with merges and/or squash commits.

> [!IMPORTANT]  
> Uncontrolled (dev box) builds should be clearly identifyable so as to not be confused with a controlled build.

> [!HINT]
> If a version is clearly from an uncontrolled build (like: `1.2.3-UNCONTROLLED`) then a build ID (in place of a build number) can be understood as used in the context of an uncontrolled build.
>
> e.g:
> ```
>     1.2.3-UNCONTROLLED.435
> ```


### Harvesting

To support later automation, like release documentation automation, it may be necessary to parse versions to extract identifiers like build number, maturity, etc.
This allows link the version to other sources like build logs and repository commits.
This is much easier with a structured scheme where an identifier can be extracted by, for example, a regular expression.

Keep the ordering and meaning of identifers consistent.


### Build number

A build number is usually an integer, that **uniquely** identifies a controlled build and alway increase in value for each build.
By convention it increments with every build. This means build versions can be sortable chronologically.

The build number provides traceability to build records and therefore is required for reproducability.

> [!IMPORTANT]
> Uncontrolled (dev box) builds cannot generate a build number. 
> To avoid confusion, consider substiting the identifier with a recongnisable value like 0 or 'U' or a build ID like 'U32'. 

See also [ControlledUncontrolled](#controlleduncontrolled).


## Versioning scheme considerations

### Objectives

It is desirable for a project's versioning scheme to provide (in recommended order of importance):

1. Sortability
1. Build maturity indication
1. Traceability
1. Reproducability
1. Independent of customer/product planned releases
1. Patch updates of prior releases
1. Low cost of ownership
1. Suitable for use

### Sortability

Use case S01:

> As a user of software artifacts I can identify the most recent release/build from a list of release/build versions.
>
> See how build maturity labels impact sorting [here](#build-maturity).

Use case S02:

> As a user of software artifacts I can use a build versions to identify the most recent build from a list of build versions.

Use case S03:

> As a user of lower maturity project artifacts the artifact's build version indicates what the artifact's intended release target was.
>
> _Is this for the next release or work to support a prior release?
> Puts testing in context & helps manage work on multiple releases.__

### Build maturity

Build maturing is often indicated using labels like:

* Release
* Release candidate (RC)
* Beta
* Alpha

The labels used will impact versions are sorted. 
On NuGet this then may impact which build a user is offered as [it searches for the highest in a list](https://learn.microsoft.com/en-us/nuget/concepts/package-versioning?tabs=semver20sort#normalized-version-numbers).

For example on NuGet:

```
    1.2.3-beta11
    1.2.3-beta10
    1.2.3-beta2

    Is ordered (listed) as:
        1.2.3-beta2       <-- appears to be the most recent (incorrect)
        1.2.3-beta10
        1.2.3-beta11
```

While:

```
    1.2.3-beta.11
    1.2.3-beta.10
    1.2.3-beta.2

    Is ordered (listed) as:
        1.2.3-beta.11   <-- appears to be the most recent (correct)
        1.2.3-beta.10
        1.2.3-beta.2
```

It is useful for more mature builds to appear higher in a list.
Fortunately alphebetical sorting of alpha, beta, and release results in ordering by maturity.

If a label like `andy-experimental` was used ordering on NuGet would be:

```
    1.2.3               <-- release
    1.2.3-alpha103
    1.2.3-beta102
    1.2.3-alpha101
    1.2.3-andy-experimental50

    Is ordered (listed) as:
        1.2.3   
        1.2.3-alpha103
        1.2.3-alpha101
        1.2.3-andy-experimental50
        1.2.3-beta102
```

And:

```
    1.2.3               <-- release
    1.2.3-alpha.103
    1.2.3-beta.102
    1.2.3-alpha.101
    1.2.3-andy-experimental.50

    Is ordered (listed) as:
        1.2.3   
        1.2.3-beta.101
        1.2.3-beta.100
        1.2.3-andy-experimental.50
        1.2.3-alpha.103
        1.2.3-alpha.102
```


### Traceability

Use cases:

> As a software developer I can identify the build in the build system's history so I can obtain build system build environment records and release notes.

> As a person testing software artifacts the artifacts version gives clear indication if it is an [uncontrolled build](#controlleduncontrolled).


### Reproducability

Reproducability is required to be able to suport prior releases and also to limit cost of testing to changes.
It also helps reduce development costs by being able to reproduce a build that had variant behaviour.
It is provided by the project's build system and code repository.

To access reproducability a version must provide traceability to its build on the build system.
 ----- TODO ----


### Patch updates of prior releases

The ability to support a prior release is often an ethical, legal, or marketing necessity.
Even if it is not, it is usually enivitable and can reduce support costs.

If versions 1.2.3, 1.2.4, and then 1.3.0 were releases, then what would be the version of a patch release of 1.2.3?

_If using Semver the answer is version 1.2.5._

> [!HINT]
> If Semver was used then the difference between releases 1.2.3 and 1.2.4 is bug fixes only.
> The version tells us that there are no new features and no breaking changes.
> Hence you would not issue a patch of 1.2.3 but a patch of 1.2.4 which is 1.2.5.
> If 1.2.n was the last 1.2.x release (before 1.3.x) then the next 1.2.x patch release would be 1.2.(n+1).


### Cost of ownership

Version scheme cost of owership is affected by:

* Compatibility with project software development methodology/processes.
* Readability - if self evident

#### Process compatibility

Process compatibility is about if the versioning reflects (and assists) a software team's actual release practices.

As example, consider how release candidates are versioned. A versioning scheme may add an "rc\<n>" label to the release version.
This is useful to allow a large number of external users to evaluate a release candidate (RC).
But, if the team never releases to external users until the product is fully tested then adding an "rc" label may introduce costs from:

* Additional "release" build to remove the label.
* Futher testing of the release build (all builds need some testing).
* A process to manage a repository if the last rc is not the one chosen to become the release.

Alternatively a team may consider all release branch builds to RCs and none have a RC label. 
One RC will eventually be selected to be the release. This is were the Semver release bump kicks in :-).

#### Readability

There is usually a churn in project developers and artifact consumers.
[Onboarding](https://en.wikipedia.org/wiki/Onboarding) costs can be considerable.
Good readability reduces ramp on costs, reduces errors, and improves acceptance.
Use of a current common industry standards helps readability.

### Suitable for use

Who is the consumer? Will the versioning be useful to them?

If doing annual or periodic releases, and future upgrade sales are important, then a [CalVer](https://calver.org/) approach may be best.
If releases are to internal consumers, like a product integration testing team, then [SemVer](https://semver.org/) may be a better fit.

## Conventions

A few of the common <a href="https://en.wikipedia.org/wiki/Software_versioning">software versioning</a> schemes:

| Name     |  Scheme / Comment    |
| :---     |  :---              |
| 3 part release version <br/>(<a href="https://semver.org/">Semantic Versioning 2.0</a>) | `<major>.<minor>.<patch>[-<prerelease>[+<metadat>]` <br/> Widely used industry defacto standard. |
| 3 part build version | `<major>.<minor>.<build>` <br/> \<build> is build number that increments on every build.     |
| 4 part build version (common) | `<major>.<minor>.<revision>.<build>` <br/>An often used convention.<br/> |
| [4 part build version (Microsoft)](https://learn.microsoft.com/en-us/dotnet/standard/assembly/versioning#assembly-version-number) | `<major>.<minor>.<build>.<revision>` <br/> Notice that the build number is the 3rd element (not last). |
| CalVer | <a href="https://calver.org/">Callendar versioning</a><br/>Very common. Feels more like a product versioning scheme. |

Semmantic versioning is highly recommended and is probably the current industry standard and best practice. 
A succint, easy to read standard with real world guideance.
It includes flexible pre-release and build metadata. 
GitHub version is based on the Semmantic Versioning standard.

----

## Sample Schema

**Note:** Here spaces are added in versions for clarity and not present in actual versions.

Objectives:

1. The objective is that all uncontrolled builds (dev box builds) are clearly identified
as different to controlled builds (build system builds). 
Important as uncontrolled build lack traceability and reproducability.

2. All controlled ``0.n.n`` builds are recognisable as [initial development](https://semver.org/#spec-item-4) builds regardless of branch.
This preserves visibility of when the first stable build (1.0.0) is achieved.

3. Versioning suitable for publishing NuGet packages to nuget.org.

4. Consise build system build version.

5. Allow using GitHub action as build system. This requires for support for using a two part build number (build number & retry count).


Version schemas here are defined by common use.


### Release versioning

| Use                  | Schema                                                            |
|:---                  |:---                                                               |
| Build system         | `1.2.3 + <build>[.<build-context>]`                                 |
| NuGet - filename     | `1.2.3`                                                           |
| NuGet - version      | `1.2.3 + <build>.<build-context>.<branch>.<short-sha>`            |
| InformationalVerion  | `1.2.3 + <build>.<build-context>.<branch>.<short-sha>`            |

There are no builds marked as an RC. No 'rc-n' metadata identifier.
This is because semantic versioning has only [one release](https://semver.org/#spec-item-3) of a [normal version](https://semver.org/#spec-item-2).
Up until the build to release is chosen all release branch builds are RCs.
In this schema it is undesirable to need to rebuild, to remove an 'rc' identifier, to release a build.


### Pre-release & initial development (0.x.x) versioning

| Use                  | Schema                                                            |
|:---                  |:---                                                               |
| Build system         | `1.2.3 - <label>.<build>[.<build-context>]`                       |
| NuGet - filename     | `1.2.3 - <label>.<build>.<build-context>`                         |
| NuGet - version      | `1.2.3 - <label>.<build>.<build-context> + <branch>.<short-sha>`  |
| InformationalVerion  | `1.2.3 - <label>.<build>.<build-context> + <branch>.<short-sha>`  |


### Pre-release labels

| Major | Label             | Controlled   | Branch  | Major  |
| :--   |:--                |:--:          |:-:               |:-:     |
| 0     | ``uncontrolled``  | false        | Any              | >= 0   |
| 0     | ``experimental``  | true         | Any              | = 0    |
| >=1   | ``beta``          | true         | Non-release      | >= 1   |

Precidence is important to ensure that the most most mature, and then recent, versions are listed first when sorted to the Semantic Version 2.0 standards.
e.g: When hosted on NuGet ensure that the most appropriate build is presented.

Label precidence:

* All ``0.n.n`` builds are prerelease ``uncontrolled`` or ``experimental`` builds.

  * Label precidence does not apply to ``uncontrolled`` builds as all uncontrolled builds use this label and they are not deployed to servers (e.g: A NuGet server).
    _Note:_ Duplicate uncontrolled build versions are possible.
  
  * Label precidence does not apply to ``experimental`` builds as all controlled 0.n.n builds use label.
  
* Here, label precidence does not apply to ``beta`` builds as that is the only possible pre-release label for builds with a ``Major`` number >= 1.
  Note: If an ``alpha`` label was used, it would have lower precidence (good).

Hence:

```
  0.2.3-uncontrolled.1001 < 0.2.3-uncontrolled.1002 < 0.2.4-uncontrolled.1000

  0.2.3-experimental.1000 < 1.0.0-beta.999 < 1.0.0 < 1.2.3
```


### Build and build context numbers

In the versioning the build and build context numbers are shown as `<build>.<build-context>`.
They are defined in the table below.

| Host                          | `<build>.<build-context>`    | Example   |
|:---                           |:---                          |:--        |
| Build system (e.g: TeamCity)  | `<build-number>.0`           | 12345.0   |
| GitHub (as a build system)    | `<run_number>.<run_attempt>` | 12345.1   |
| Any uncontrolled host         | `<count>.<machine-name>`     | 1234.MyPC |

GitHub's `github.run_attempt` starts at 1.

### Examples

| Version                         | Description                  |
|:---                             |:---                          |
| 0.4.5-experimental.47892.0      | An early development build system (other than GitHub) build.<br/>Build number 47892. |
| 0.6.3-uncontrolled.334.JohnsPC  | A developers box (JohnsPC) build that has a local build count of 334. <br/>An uncontrolled build. |
| 1.2.3                           | An RC build or the 1.2.3 release build. Made on a release branch on the build system.<br/>A controlled build. |
| 1.2.3+479002.0.main.a53f5678    | The information version forthe 1.2.3 RC/release build.<br/>A controlled build. |
| 1.2.3-beta.47900.0              | Build system pre-release build made on the build system.<br/>Build number 47900.<br>A controlled build.
| 1.2.3-uncontrolled.338.JohnsPC  | Another uncontrolled build from JohnsPC.<br/>Local build count 338.<br>A controlled build. |
| 1.2.3-beta.47900.1              | Build system pre-release build made on a GitHub build system.<br/>Run number 47900. Run attempt number 1.<br>A controlled build.
