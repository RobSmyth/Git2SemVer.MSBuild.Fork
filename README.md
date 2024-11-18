![](Documentation/Images/Git2SemVer_banner_840x70.png)

# GitSemVer

Automatic true [Semmantic Versioning](https://semver.org/) for both **Visual Studio** and donet CLI builds of .NET solutions and projects.
Provides versioning _out of box_. It is fully customisable by by built-in C# scripting and versioning API.

Just works on every <b>Visual Studio</b> or dotnet CLI build on every box.

Go to [documentation](https://noetictools.github.io/Git2SemVer.MSBuild/) for more information.


## Features

* _Out of box_ automatic true [Semmantic Versioning](https://semver.org/).
* **No limits** customisable by built-in C# scripting and versioning API.
* Every build (& rebuild) is traceable (default versioning):
  * Incrementing build number on each build system or developer's box build or rebuild (not reliant on commit depth).
  * Version identifies its build host as build system or developer's box and which developer's box.
  * Automatically uses build system (e.g: TeamCity) provided build number.
* No extra build system version generation step or tools.
* Automatic versioning of developer and build system builds in both **Visual Studio** and donet CLI builds. Just build.
* Uses [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) message elements to manage version.
* Branching model (such as GitHub Flow or GitFlow) agnostic. No model configuration.
* Easy solution setup with dotnet tool Git2SemVer.Tool.

See more about features [here](https://noetictools.github.io/Git2SemVer.MSBuild).


## Why use? 

Use if you want:

* [Environment parity](Documentation/Glossary.md#environment-parity) - Same build on any host, fewer environment tools. Reduces maintenance costs.
* Entirely specialised versioning like: `<year>.<moon phase>.<local surf conditions>`
* Build traceability - Build build numbering on all builds including developer box builds. Commit height is too unreliable for you.
* Semmantic Versioning _release_ versioning.
* To use Visual Studio as well as dotnet CLI.


## License

`Git2SemVer` uses the [MIT license](https://choosealicense.com/licenses/mit/).

## Acknowledgments

This project uses the following tools and libraries. Many thanks to those who created and manage them.

* [Spectre.Console](https://github.com/spectreconsole/spectre.console)
* [Injectio](https://github.com/loresoft/Injectio)
* [JetBrains Annotations](https://www.jetbrains.com/help/resharper/Code_Analysis__Code_Annotations.html)
* [Semver](https://www.nuget.org/packages/Semver) - files copied to create subset
* [NuGet.Versioning](https://www.nuget.org/packages/NuGet.Versioning)
* [NUnit](https://www.nuget.org/packages/NUnit)
* [Moq](https://github.com/devlooped/moq)
* [docfx](https://dotnet.github.io/docfx/)
* <a href="https://www.flaticon.com/free-icons/brain" title="brain icons">Brain icons created by Freepik - Flaticon</a>

