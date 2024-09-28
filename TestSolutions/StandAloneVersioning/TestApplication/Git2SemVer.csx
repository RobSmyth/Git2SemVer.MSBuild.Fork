using System;
using Semver;
using Microsoft.Build.Utilities;
using NoeticTools.Git2SemVer.MSBuild;
using NoeticTools.Git2SemVer.MSBuild.Framework.Semver;
using NoeticTools.Git2SemVer.MSBuild.Scripting;


var context = VersioningContext.Instance!;
