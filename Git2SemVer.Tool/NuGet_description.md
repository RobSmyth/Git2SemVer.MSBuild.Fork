# Git2SemVer.Tool

Git2SemVer.Tool is a dotnet tool used to setup [Git2SemVer](https://github.com/NoeticTools/Git2SemVer) solution versioning in a .NET solution.

Go to [documentation](https://noetictools.github.io/Git2SemVer/) for more information.

To run, first install:

```
dotnet tool install --global NoeticTools.Git2SemVer.Tool
```

Then, in the solution's directory, run:

```
Git2SemVer add
```

You will be prompted with a few options and then it is done.

* No extra build steps
* No build scripts
* No change to build system
* No extra build environment tools
* Automatic versioning of developer and build system builds in both Visual Studio and donet CLI builds

Just works. The versioning it is fully customisable, see [Git2SemVer](https://github.com/NoeticTools/Git2SemVer).

**NOTE**
> Git2SemVer.Tool is only required for solution setup.
> It is not required at compile time and does not need to become part of the development environment.
