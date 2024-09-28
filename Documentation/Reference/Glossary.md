---
uid: glossary
---
![](../Images/Git2SemVer_banner_840x70.png)

# Glossary


## Build ID

A build ID is one or more identifiers used when a build number is not available ([uncontrolled builds](#uncontrolled-build)).
A build ID is intended to provide a possibility of traceability.

So that build IDs are not confused with build numbers it is useful for version prerelease and/or metadata to identify uncontrolled builds.

## Build height

Build height refers to a count of commits since a reference commit.

## Build number

A build number uniquely identifies a build to provide traceability to build logs/records on a build system.
Traditionally a integer that increases with each build.
It is useful if the build number always increases chronologically.

A build number is critical to achieve traceability and therefore also to acheive reproducability.

[Uncontrolled builds](#uncontrolled-build) cannot provide a build number to this definition and instead provide a non-traceable "build ID".

**Note:** A semmantic version usually requires a build number as identifiers including the normal version (1.2.3),
commit ID (SHA), and commits count are not unique.

## Controlled build

See [Controlled host](#controlled-host)

## Controlled host

A controlled host is a build host that:

* Has a controlled environment so that builds on the host are reproducable.
* Keeps build logs.
* Keeps build records of build environment.

A build system is normally a controlled host.

## Environment parity

Environment parity is a best pracitice.
It is the practise of keeping all dev/production environments as similar as possible.

## Initial development

A Semantic Versioning 2.0 version with a major version number of 0 is an [initial development](https://semver.org/#spec-item-4) build.
This refers to a period of software development from project start to the first stable release.

## Metadata identifier

See Semantic Versioning 2.0 description of [build metadata](https://semver.org/#spec-item-10).

## Prerelease identifier

See Semantic Versioning 2.0 description of a [prerelease version](https://semver.org/#spec-item-9).

## Prerelease label

A prerelease label, like "beta", is a category describing a prerelease's maturity.
For prereleases to be ordered by maturing (after normal semver) it is the first prerelease identify to give [Semantic Versioning precidence](https://semver.org/#spec-item-11) by maturity.

For the most mature prerelease to be shown first when sorted lexically in ASCII sort order.
For example a beta build is more mature than an alpha build and `"beta" > "alpha"`.

## Semver

An abbreviation for [Semantic Versioning](https://semver.org/).

## Uncontrolled build

See [Uncontrolled host](#uncontrolled-host)

## Uncontrolled host

An uncontrolled host is a build host that is not a [controlled host](#controlled-host).
That is, at the time of a build, it is not known for certain what build tools, source code, and OS version was used.
Build logs and records are not kept.

Builds from an uncontrolled host may not be reproducable and the build environment may have unknown differences to the build production environment.

Typically developer boxes are uncontrolled hosts.

**Note:** Uncontrolled hosts normally do not provide a the build with a [build number](#build-number).