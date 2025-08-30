---
uid: changelog-data-files
---

# Changelog - Data files

**Git2SemVer** changelog generated data files are found in a folder set by the 
**Git2SemVer.Tool** [DataDirectory](xref:git2semver-tool-commands) (default is `.git2semv/changelog`). 
If this folder does not exist **Git2SemVer** will create it.

The files found in this folder are:

| Filename                             | Description                                                         |
| :---                                 | :---                                                                |
| `git2semver.changelog.settings.json` | [Generator settings file](xref:changelog-settings).                 |
| `markdown.template.scriban`          | The scriban template the generator uses to generate the changelog.  |
| `<CHANGELOG>.md.data.g.json`    | The generator's last run data for changelog (`<CHANGELOG>.md`). Includes all [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) that have been handled.  |

Include all of these files in the Git repository.
