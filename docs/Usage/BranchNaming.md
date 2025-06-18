﻿---
uid: branch-naming
---

<div style="background-color:#944248;padding:0px;margin-bottom:0.5em">
  <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/Git2SemVer_banner_840x70.png"/>
</div>

[![Current Version](https://img.shields.io/nuget/v/NoeticTools.Git2SemVer.MSBuild?label=Git2SemVer.MSBuild)](https://www.nuget.org/packages/NoeticTools.Git2SemVer.MsBuild)
<a href="https://github.com/NoeticTools/Git2SemVer">
  ![Static Badge](https://img.shields.io/badge/GitHub%20project-944248?logo=github)
</a>


# Branch naming

A _branch maturity pattern_ is used to identify if a build in a branch is a release or pre-release build and 
the [_build maturity identifier_](xref:maturity-identifier) (e.g: rc, alpha, beta). This topic documents the default branch maturity pattern and
how to use custom patterns.

## Default branch maturity pattern

The default branch maturity pattern regular expression pattern is:

```xml
^((?<rc>(main|release)[\\\/_](.*[\\\/_])?rc.*)|(?<release>main|release)|(?<beta>feature)|(?<alpha>.+))[\\\/_]?
```

The table below gives example default release/pre-release branch [versioning](xref:versioning) (SHAs abbreviated).

| Git branch                                | Type                | Example product version  | Example informational version           |
|:--                                        |:--                  |:--                       |:--                                      |
| `main`<br/>`release`<br/>`release/apples` | Release             | 1.2.3                    | 1.2.3+801.\<branchName>.e10c9cc0        |
| `release_rc`<br/>`release/rc`             | Pre-release (RC)    | 1.2.3-rc.801             | 1.2.3-rc.801+\<branchName>.e10c9cc0     |
| `feature`<br/>`feature/oranges`           | Pre-release (Beta)  | 1.2.3-beta.801           | 1.2.3-beta.801+\<branchName>.e10c9cc0   |
| Others not matching                       | Pre-release (Alpha) | 1.2.3-alpha.801          | 1.2.3-alpha.801+\<branchName>.e10c9cc0  |


## Custom configuration

A custom branch maturity pattern regular expression pattern can be set using the [`Git2SemVer_BranchMaturityPattern`](xref:msbuild-properties).

For example:
```xml
<PropertyGroup>
    <Git2SemVer_BranchMaturityPattern>
        ^((?<rc>(main|release)[\\\/_](.*[\\\/_])?rc.*)|(?<release>main|release)|(?<beta>feature)|(?<alpha>.+))[\\\/_]?
    </Git2SemVer_BranchMaturityPattern>
</PropertyGroup>
```

The regular expression must have two or more [named groups](https://learn.microsoft.com/en-us/dotnet/standard/base-types/grouping-constructs-in-regular-expressions#named-matched-subexpressions).
The `release` named group is required and defines which branches are release branches. 
All other named groups define pre-release branches and the group's name is the [build maturity identifier](xref:maturity-identifier).
The last named group must be a catch all (e.g: `(?<alpha>.+)`) for pre-release branches not matching prior groups.

For example, a branch maturity pattern for a single release branch `main` with all other branches being pre-releases `dev` branches is:
```xml
^((?<release>main)|(?<dev>.+))[\\\/_]?
```
This results in builds on a branch like `mystuff/oranges` having pre-release [versioning](xref:versioning) like `1.2.3-dev.801+<branchName>.e10c9cc0`.

> [!NOTE]
> When a branch name or maturity identifier is used in version metadata, [invalid Semmmantic Versioning characters](https://semver.org/#spec-item-10) are replaced with the "-" characters.
> This is to ensure Semmmantic Versioning compliance for compatibility with common tools.


## Related topics

* [Release tagging](xref:release-tagging)
* [Versioning](xref:versioning)
* [Build maturity identifier](xref:maturity-identifier)
* [Build properties](xref:msbuild-properties)
