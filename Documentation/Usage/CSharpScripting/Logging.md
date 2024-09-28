---
uid: script-logging
---
![](../../Images/Git2SemVer_banner_840x70.png)

# C# Script Logging

Log messages are are seen in the compiler output.

The C# script uses a [ITaskLogger](xref:NoeticTools.Git2SemVer.Framework.Logging.ITaskLogger) logger instance
available on the [ScriptRunner's Logger](xref:NoeticTools.Git2SemVer.IScriptRunner.Logger) property.
This logger is available in both the script's global and class contexts.

The C# script is provided with a [logger](xref:NoeticTools.Git2SemVer.Framework.Logging.ITaskLogger)
can log is provided with a [Logger](xref:NoeticTools.Git2SemVer.IScriptRunner.Logger).
This logger is available in both the global and class contexts.


# [With runner instance (recommended)](#tab/Object)

```csharp
var runner = ScriptRunner.Instance!;
runner.Logger.LogImportantMessage("Hellow world);
```

# [Global context](#tab/Global)

```csharp
Logger.LogImportantMessage("Hellow world);
```

---
