
# Git2SemVer.MSBuild Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


## 3.3.0 - _July 25, 2025_

### Added

* Add versioning waypoint tag support(#55).
* Add option to set release tag format (#63).
* Support Conventional Commits multiline footers (#64).

### Changed

None.

### Fixed

None.

## 3.2.0 - _June 23, 2025_

### Added

* Improved versioning algorithm that improves reduces version time perfromance. Faster on complex git trees.

### Changed

None.

### Fixed

None.

## 3.1.0 - _June 18, 2025_

### Added

* Build property to enable customising the release tag format ([#37](https://github.com/NoeticTools/Git2SemVer.MSBuild/issues/37)).

### Changed

None.

### Fixed

None.

## 3.0.0 - _June 15, 2025_

Git2SemVer.MSBuild 3.0.0 has breaking changes:

* The TeamCity version build time statistic was renamed from to `Git2SemVerRunTime_sec` to `git2semver.runtime.seconds`.
* Builds on Linux hosts (e.g: Ubuntu >= 22) may break with an error that `libdl.so` was not found.

For futher information see: [Upgrading V2 to V3](https://noetictools.github.io/Git2SemVer.MSBuild/Reference/Upgrading/UpgradingV2toV3).

### Added

* Warning if .net sdk 8.0 adding git SHA suffix to versions ([#43](https://github.com/NoeticTools/Git2SemVer.MSBuild/issues/43)).
* Further information in logs/report as to how the version is calculated ([#42](https://github.com/NoeticTools/Git2SemVer.MSBuild/issues/42)).
* Add warning and error IDs for MSBuild integration ([#41](https://github.com/NoeticTools/Git2SemVer.MSBuild/issues/41)).

### Changed

* **Breaking change -** Improve build statistic naming  ([#44](https://github.com/NoeticTools/Git2SemVer.MSBuild/issues/44)).

### Fixed

* Unable to parse git --version response: '' ([#40](https://github.com/NoeticTools/Git2SemVer.MSBuild/issues/40)).
* Breaking change footnote is not bumping version ([#38](https://github.com/NoeticTools/Git2SemVer.MSBuild/issues/38)).
* A metadata identifier can contain only ASCII alphanumeric characters and hyphens ' (Apple Git-154)' ([#35](https://github.com/NoeticTools/Git2SemVer.MSBuild/issues/35)).


## 2.0.0 - _December 1, 2024_

This release has breaking changes that will impact users that are either:
* Using the generated JSON `Git2SemVer.VersionInfo.g.json` file in custom code.
* Using the Git API in custom C# script (csx) code.

### Added

* C# scripting (csx):
  * `IGitTool` fluent API added ([#5](https://github.com/NoeticTools/Git2SemVer.Core/issues/5)).
  * `IGitTool` functionality to get commits between revision ranges ([#5](https://github.com/NoeticTools/Git2SemVer.Core/issues/5)).

### Changed

* **Breaking change** to generated JSON file `Git2SemVer.VersionInfo.g.json`. These will only impact users using this file externally.
    * CommitId `Id` property renamed to `Sha`.
    * `CommitChangeTypeId` enumb `None` value (1) added. All higher numbers bumped.
    * `Rev` bumped to `2`.
    * Others
* **Breaking change** `IGitTool` method signatures changed. Introduced Fluent API.
* TeamCity automatic build label updating now adds build number metadata to release build label, e.g: 1.2.3+34567 ([#33](https://github.com/NoeticTools/Git2SemVer/issues/33)). 

### Fixed

* On a new repo without any release tags, conventional commit version bumps are ignored ([#34](https://github.com/NoeticTools/Git2SemVer.MSBuild/issues/34)).


## 1.2.0 - _November 26, 2024_

### Changed

* Changed the build system label, as used by TeamCity, to include build number on release builds ([#33](https://github.com/NoeticTools/Git2SemVer.MSBuild/issues/33)).
* Moved Git2SemVer.Tool (dotnet tool) code to its own repository.

### Known Issues

* Versioning fails build when TeamCity build configuration's VCS Root uses the option "Use tags as branches" is set ([#32](https://github.com/NoeticTools/Git2SemVer.MSBuild/issues/32)). A compile time exception is thrown.

 
## 1.1.0 - _November 16, 2024_

### Added

* Added [Cross targeted projects](https://learn.microsoft.com/en-us/nuget/create-packages/multiple-target-frameworks-project-file) support ([#26](https://github.com/NoeticTools/Git2SemVer/issues/26)).
* Added versioning log file ([#26](https://github.com/NoeticTools/Git2SemVer/issues/2)).
* Added [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0-beta.2/) support for versioning.

### Fixed

* On an uncontrolled host with solution open in Visual Studio, build number updates in background every few seconds ([#28](https://github.com/NoeticTools/Git2SemVer/issues/28)).
* On an uncontrolled host prior build number is reused on first build after new clone ([#27](https://github.com/NoeticTools/Git2SemVer/issues/27)).
* ([#24](https://github.com/NoeticTools/Git2SemVer/issues/24)).


### Known Issues

* Solution configured using `Git2SemVer.Tool` dotnet tool 1.0.0 does not allow for `Git2SemVer.MSBuild` package. Requires manual correction ([#25](https://github.com/NoeticTools/Git2SemVer/issues/25)).


## 1.0.0 - _November 7, 2024_

First release.

### Added

* Visual Studio & dotnet CLI solution Semmantic versioning
* Build hosts:
  * TeamCity - detects build number and updates build label
  * GitHub - supports composite build numbering
  * Uncontrolled (dev box) - local build numbering
* C# scripting
* Git workflow agnostic

### Known Issues

* Does not work with [cross targeted projects](https://learn.microsoft.com/en-us/nuget/create-packages/multiple-target-frameworks-project-file).
See issue [#26](https://github.com/NoeticTools/Git2SemVer/issues/26).
