import jetbrains.buildServer.configs.kotlin.*
import jetbrains.buildServer.configs.kotlin.buildFeatures.perfmon
import jetbrains.buildServer.configs.kotlin.buildSteps.dotnetBuild
import jetbrains.buildServer.configs.kotlin.buildSteps.dotnetNugetPush
import jetbrains.buildServer.configs.kotlin.buildSteps.dotnetRestore
import jetbrains.buildServer.configs.kotlin.buildSteps.dotnetTest
import jetbrains.buildServer.configs.kotlin.buildSteps.script
import jetbrains.buildServer.configs.kotlin.failureConditions.BuildFailureOnMetric
import jetbrains.buildServer.configs.kotlin.failureConditions.BuildFailureOnText
import jetbrains.buildServer.configs.kotlin.failureConditions.failOnMetricChange
import jetbrains.buildServer.configs.kotlin.failureConditions.failOnText
import jetbrains.buildServer.configs.kotlin.triggers.vcs

/*
The settings script is an entry point for defining a TeamCity
project hierarchy. The script should contain a single call to the
project() function with a Project instance or an init function as
an argument.

VcsRoots, BuildTypes, Templates, and subprojects can be
registered inside the project using the vcsRoot(), buildType(),
template(), and subProject() methods respectively.

To debug settings scripts in command-line, run the

    mvnDebug org.jetbrains.teamcity:teamcity-configs-maven-plugin:generate

command and attach your debugger to the port 8000.

To debug in IntelliJ Idea, open the 'Maven Projects' tool window (View
-> Tool Windows -> Maven Projects), find the generate task node
(Plugins -> teamcity-configs -> teamcity-configs:generate), the
'Debug' option is available in the context menu for the task.
*/

version = "2025.03"

project {

    buildType(BuildAndTest)
    buildType(DeployLocalTeamCityPackage)

    cleanup {
        baseRule {
            option("disableCleanupPolicies", true)
        }
    }
}

object BuildAndTest : BuildType({
    name = "Build and test"

    artifactRules = """
        +:MSBuild/nupkg/NoeticTools.*.nupkg
        +:SolutionVersioningProject/obj/Git2SemVer.MSBuild.log
        +:SolutionVersioningProject/.git2semver/Git2SemVer.VersionInfo.g.json
    """.trimIndent()

    params {
        param("BuildConfiguration", "Release")
    }

    vcs {
        root(DslContext.settingsRoot)

        cleanCheckout = true
    }

    steps {
        script {
            name = "Clear NuGet caches"
            id = "Clear_NuGet_caches"
            scriptContent = "dotnet nuget locals all --clear"
        }
        dotnetRestore {
            name = "Restore"
            id = "Restore"
            sources = """
                https://api.nuget.org/v3/index.json
                http://10.1.10.78:8111/guestAuth/app/nuget/feed/_Root/TeamCity/v3/index.json
            """.trimIndent()
        }
        dotnetBuild {
            name = "Build"
            id = "dotnet"
            configuration = "%BuildConfiguration%"
            args = "-p:Git2SemVer_BuildNumber=%build.number% --verbosity normal"
        }
        dotnetTest {
            name = "Test"
            id = "dotnet_1"
            configuration = "%BuildConfiguration%"
            skipBuild = true
            args = "--logger console --logger teamcity"
            param("dotNetCoverage.dotCover.filters", """
                +:NoeticTools.*
                -:NoeticTools.*Tests
            """.trimIndent())
        }
    }

    triggers {
        vcs {
        }
    }

    failureConditions {
        executionTimeoutMin = 3
        failOnMetricChange {
            enabled = false
            metric = BuildFailureOnMetric.MetricType.TEST_COUNT
            threshold = 20
            units = BuildFailureOnMetric.MetricUnit.PERCENTS
            comparison = BuildFailureOnMetric.MetricComparison.LESS
            compareTo = build {
                buildRule = lastSuccessful()
            }
        }
        failOnMetricChange {
            metric = BuildFailureOnMetric.MetricType.ARTIFACT_SIZE
            threshold = 25
            units = BuildFailureOnMetric.MetricUnit.PERCENTS
            comparison = BuildFailureOnMetric.MetricComparison.LESS
            compareTo = build {
                buildRule = lastSuccessful()
            }
        }
        failOnText {
            conditionType = BuildFailureOnText.ConditionType.CONTAINS
            pattern = "The service message is invalid because it does not end with ] character"
            failureMessage = "Service message corruption detected"
            reverse = false
            stopBuildOnFailure = true
        }
    }

    features {
        perfmon {
        }
    }

    requirements {
        exists("DotNetCLI_Path")
    }
})

object DeployLocalTeamCityPackage : BuildType({
    name = "Deploy (local TeamCity) - package"
    description = "Deploy NuGet package"

    enablePersonalBuilds = false
    type = BuildTypeSettings.Type.DEPLOYMENT
    buildNumberPattern = "%build.counter% (${BuildAndTest.depParamRefs.buildNumber})"
    maxRunningBuilds = 1

    steps {
        dotnetNugetPush {
            name = "Push NuGet package"
            id = "Publish2"
            packages = "NoeticTools.*.nupkg"
            serverUrl = "http://10.1.10.78:8111/guestAuth/app/nuget/feed/RobSmyth/TeamCity/v3/index.json"
            apiKey = "credentialsJSON:bd18b974-1188-423d-9efd-8836806c3669"
        }
    }

    dependencies {
        artifacts(BuildAndTest) {
            buildRule = lastSuccessful("""
                +:<default>
                +:*
            """.trimIndent())
            cleanDestination = true
            artifactRules = "+:NoeticTools.Git2SemVer.MSBuild.*.nupkg"
        }
    }
})
