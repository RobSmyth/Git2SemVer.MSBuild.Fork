---
uid: script-logging
---
![](../../Images/Git2SemVer_banner_840x70.png)

# C# Script Logging

Log messages are are seen in the compiler output.

The C# script uses a [ITaskLogger](xref:NoeticTools.Common.Logging.ILogger) logger instance
available on the ScriptRunner's context [Logger](xref:NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.Builders.Scripting.VersioningContext.Logger) property.
This logger is available in both the script's global and class contexts.


## With runner instance (recommended)

```csharp
var runner = ScriptRunner.Instance!;
runner.Logger.LogImportantMessage("Hellow world);
```

## Global context

```csharp
Logger.LogImportantMessage("Hellow world);
```

---
