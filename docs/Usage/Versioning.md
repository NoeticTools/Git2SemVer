﻿---
uid: versioning
---

<div style="background-color:#944248;padding:0px;margin-bottom:0.5em">
  <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/Git2SemVer_banner_840x70.png"/>
</div>

[![Current Version](https://img.shields.io/nuget/v/NoeticTools.Git2SemVer.MSBuild?label=Git2SemVer.MSBuild)](https://www.nuget.org/packages/NoeticTools.Git2SemVer.MsBuild)
<a href="https://github.com/NoeticTools/Git2SemVer">
  ![Static Badge](https://img.shields.io/badge/GitHub%20project-944248?logo=github)
</a>

# Versioning

Git2SemVer generates versioning information and passes this to the optional [C# script](xref:csharp-script) for modification before being passed back to the build.

![](../Images/MSBuild_tasks_01.png)

This topic describes the default versioning without a custom C# script and without customised [build properties](xref:msbuild-properties).

## Release versioning

Builds on [release branches](xref:branch-naming) use release versioning.
The default release versioning format is:

```
  <major>.<minor>.<patch>[+<build-id>.<branch>.<commit-sha>]
```

Where:

* `<build-id>` is the [build ID or build number](xref:build-id).
* `<branch>` is the [branch name](xref:branch-naming).
* `<commit-sha>` is the [git commit SHA](xref:commit-sha).

Examples:

| Use                   | Schema                                           |
|:---                   |:---                                              |
| Build system          | `1.2.3+<build-id>`                               |
| NuGet - filename      | `1.2.3`                                          |
| Informational verion  | `1.2.3+<build-id>.<branch>.<commit-sha>`         |

> [!IMPORTANT]  
> All [initial development](https://semver.org/#spec-item-4) versions (0.x.x) are pre-releases.

## Pre-release versioning

Builds on [pre-release branches](xref:branch-naming) use release versioning.
The default pre-release versioning format is:

```
  <major>.<minor>.<patch>-<maturity-label>.<build-id>[+<branch>.<commit-sha>]
```

Where:

* `<maturity-label>` is the [pre-release maturity label](xref:maturity-identifier).
* `<build-id>` is the [build ID or build number](xref:build-id).
* `<branch>` is the [branch name](xref:branch-naming).
* `<commit-sha>` is the [git commit SHA](xref:commit-sha).

Examples:

| Use                   | Schema                                           |
|:---                   |:---                                              |
| Build system          | `1.2.3-<label>.<build-id>`                       |
| NuGet - filename      | `1.2.3-<label>.<build-id>`                       |
| Informational verion  | `1.2.3-<label>.<build-id>+<branch>.<commit-sha>` |


## Initial development versioning (0.x.x)

All versions with a 0 major number (0.x.x) are initial development build versions (See [Semmantic Versioning spec item 4](https://semver.org/#spec-item-4)).
Git2SemVer makes all initial development builds pre-release build with "InidialDev" added to the [maturity ID](xref:maturity-identifier) as shown in
the table below.

### [Initial development builds](#tab/initial-dev-builds)

| Branch type      | Maturity Label     |
|:---              |:--                 |
| Release          | `InitialDev`       |
| RC               | `rc-InitialDev`    |
| Feature          | `beta-InitialDev`  |
| Development      | `alpha-InitialDev` |

### [Builds with Major >= 1](#tab/post-initial-dev-builds)

| Branch type      | Maturity Label     |
|:---              |:--                 |
| Release          | none               |
| RC               | `rc`               |
| Feature          | `beta`             |
| Development      | `alpha`            |

---

> [!NOTE]
> Pre-release maturity labels are configurable by setting the MSBuild property [Git2SemVer_BranchMaturityPattern](xref:msbuild-properties).
> Values shown here are defaults.

## Examples

See [Versioning examples](xref:examples).


## Related topics

* [Workflow](xref:workflow)
* [Release tagging](xref:release-tagging)
* [Branch naming](xref:branch-naming)
* [Build maturity identifier](xref:maturity-identifier)
* [Build properties](xref:msbuild-properties)
