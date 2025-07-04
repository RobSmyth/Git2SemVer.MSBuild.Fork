﻿using System;
using Microsoft.Build.Utilities;
using NoeticTools.Git2SemVer.Framework;
using NoeticTools.Git2SemVer.Framework.Framework.Semver;
using NoeticTools.Git2SemVer.Framework.Generation.Builders.Scripting;
using Semver;

var context = VersioningContext.Instance!;

context.Logger.LogInfo("Running script ForceProperties1.csx.");

// Make sure context properties are accessible here
var x = context.Inputs.Version;
var y = context.Host.BuildNumber;
var z = context.Outputs.Git.BranchName;

// force outputs, making each different.
// and the assembly versions can be set event if Version is set.
context.Outputs.Version = new SemVersion(11, 12, 13).WithPrerelease("a-prerelease");
context.Outputs.InformationalVersion = context.Outputs.Version.WithMetadata("metadata");
context.Outputs.AssemblyVersion = new Version(1, 2, 3);
context.Outputs.FileVersion = new Version(4, 5, 6);
context.Outputs.PackageVersion = new SemVersion(5, 6, 7);

// Note: Build system version not set to avoid chaning build number when running the tests
//context.Outputs.BuildSystemVersion = new SemVersion(1, 2, 3).WithPrerelease("alpha");

context.Logger.LogInfo($"Forced informational version to: {context.Outputs.InformationalVersion}");
context.Logger.LogInfo($"Forced assembly version to:      {context.Outputs.AssemblyVersion}");

/*
 * Expected outcome:
 *
 *  Assembly version:       1.2.3.0
 *  File version:           4.5.6
 *  Informational version:  11.12.13-a-prerelease+metadata
 *  Product version:        11.12.13-a-prerelease+metadata
 *
 *  Nuget package 4.6.7
 */