﻿---
uid: examples
---

<div style="background-color:#944248;padding:0px;margin-bottom:0.5em">
  <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/Git2SemVer_banner_840x70.png"/>
</div>

[![Current Version](https://img.shields.io/nuget/v/NoeticTools.Git2SemVer.MSBuild?label=Git2SemVer.MSBuild)](https://www.nuget.org/packages/NoeticTools.Git2SemVer.MsBuild)
<a href="https://github.com/NoeticTools/Git2SemVer">
  ![Static Badge](https://img.shields.io/badge/GitHub%20project-944248?logo=github)
</a>

# Version examples

Default versioning examples.

## Releases

### [TeamCity builds](#tab/controlled-build-teamcity)

| Version               | Example          |
|:---                   |:---                             |
| Build system          | `1.2.3+7658`                    |
| NuGet - filename      | `1.2.3`                         |
| Informational version | `1.2.3+7658.release-v1.34e6a01` |

### [GitHub builds](#tab/controlled-build-github)

| Version               | Example         |
|:---                   |:---                               |
| Build system          | `1.2.3+7658.1`                    |
| NuGet - filename      | `1.2.3`                           |
| Informational version | `1.2.3+7658.1.release-v1.34e6a01` |

### [Uncontrolled builds](#tab/uncontrolled-build)

| Version               | Example          |
|:---                   |:---                                       |
| Build system          | `1.2.3+DevPCName.7658`                    |
| NuGet - filename      | `1.2.3`                                   |
| Informational version | `1.2.3+DevPCName.7658.release-v1.34e6a01` |

---

## Prereleases

### [TeamCity builds](#tab/controlled-build-teamcity)

[TeamCity](xref:teamcity) build versions.

The versions below assume this is `beta`.

| Version               | Example                                |
|:---                   |:---                                         |
| Build system          | `1.2.3-beta.7658`                           |
| NuGet - filename      | `1.2.3-beta.7658`                           |
| Informational version | `1.2.3-beta.7658+feature-mybranch.6ab397d5` |

### [GitHub builds](#tab/controlled-build-github)

[GitHub worfklow](xref:github-workflows) build versions.

The versions below assume this is `beta`.

| Version | Example                                           |
|:---                   |:---                                           |
| Build system          | `1.2.3-beta.7658.1`                           |
| NuGet - filename      | `1.2.3-beta.7658.1`                           |
| Informational version | `1.2.3-beta.7658.1+feature-mybranch.6ab397d5` |

### [Uncontrolled builds](#tab/uncontrolled-build)

[Uncontrolled (dev environment)](xref:uncontrolled-host) builds. Uses host's managed build number and the machine name as the build context.

The versions below assume a host machine name of `DevPCName`.

| Version | Example                                           |
|:---                  |:---                                                       |
| Build system         | `1.2.3-beta.DevPCName.212`                            |
| NuGet - filename     | `1.2.3-beta.DevPCName.212`                            |
| Informational version | `1.2.3-beta.DevPCName.212+feature-mybranch.6ab397d5`  |

---

