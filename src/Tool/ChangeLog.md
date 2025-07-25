<!--
  This markdown changelog was generated by Git2SemVer.Changelog tool and lists changes since last release.
  
  It contains named sections, like (with '--' replaced with '-' in html comment tag shown here to avoid nested comments):
  
    <!- Section start: version ->
       :
    <!- Section end: version ->

  Any manual changes in the 'version' and '<name> - to groom' sections may be lost on the next tool run.
  When a change is groomed, move it out of the `- to groom` section.
  This also helps to indicate which changes have not been groomed.

  Note: Do not remove any `<!- Section .... ->` section markers, even if they look empty.
-->
# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


<!-- Section start: next release -->

<!-- Section start: version -->
## Unreleased

<!-- Section end: version -->

### Added

<!-- Tip: Once a change is groomed consider moving it above this line to separate groomed and ungroomed changes. -->
<!-- Section start: Added - changes -->
* Changelog generator (#57).
* Versioning waypoint tagging (#55).
* [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) multiline footer topic values support (#64).
* Versioning options for release tag format and branch maturity pattern (#65).
<!-- Section end: Added - changes -->

### Changed

<!-- Tip: manually remove following 'none' text when first changes are found/groomed -->
None.

<!-- Tip: Once a change is groomed consider moving it above this line to separate groomed and ungroomed changes. -->
<!-- Section start: Changed - changes -->
<!-- Section end: Changed - changes -->

### Depreciated

<!-- Tip: manually remove following 'none' text when first changes are found/groomed -->
* The `git2semver add/remove/run` commands no longer appear in command line help.
They remain functional but will be removed in a future release.
Replaced by `git2semver versioning run` and `git2semver versioning setup add/remove`.

<!-- Tip: Once a change is groomed consider moving it above this line to separate groomed and ungroomed changes. -->
<!-- Section start: Depreciated - changes -->
<!-- Section end: Depreciated - changes -->

### Removed

<!-- Tip: manually remove following 'none' text when first changes are found/groomed -->
None.

<!-- Tip: Once a change is groomed consider moving it above this line to separate groomed and ungroomed changes. -->
<!-- Section start: Removed - changes -->
<!-- Section end: Removed - changes -->

### Fixed

<!-- Tip: manually remove following 'none' text when first changes are found/groomed -->
None.

<!-- Tip: Once a change is groomed consider moving it above this line to separate groomed and ungroomed changes. -->
<!-- Section start: Fixed - changes -->
<!-- Section end: Fixed - changes -->

### Security

<!-- Tip: manually remove following 'none' text when first changes are found/groomed -->
None.

<!-- Tip: Once a change is groomed consider moving it above this line to separate groomed and ungroomed changes. -->
<!-- Section start: Security - changes -->
<!-- Section end: Security - changes -->

### Other

<!-- Tip: manually remove following 'none' text when first changes are found/groomed -->
None.

<!-- Tip: Once a change is groomed consider moving it above this line to separate groomed and ungroomed changes. -->
<!-- Section start: Other - changes -->
<!-- Section end: Other - changes -->

<!-- Section end: next release -->

## 1.1.0 - _December 3, 2024_

### Added

* Added `Run` command to run Git2SemVer version generator in current directory.


## 1.0.2 - _November 18, 2024_

A maintenance release. Added self versioning.


## 1.0.1 - _November 8, 2024_


## 1.0.0 - _November 7, 2024_

First release.
