---
_layout: landing
---
<style>

.featureTitle {
  font-size:1.2em;
  font-weight:bold;
}

.iconcolumn {
  width:15%;
  text-align:center;
}

.featureBody {
  font-size:1.0em;
}

.featureBodyLeftAlign {
  font-size:1.0em;
  text-align:left;
}

table, tr {
  border:none !important;
}

td {
  border:none !important;
  width:300px;
}

a 
{
  text-decoration: none; 
}
</style>

<div style="background-color:#944248;padding:0px;margin-bottom:0.5em">
  <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/Git2SemVer_banner_840x70.png"/>
</div>

[![Current Version](https://img.shields.io/nuget/v/NoeticTools.Git2SemVer.MSBuild?label=Git2SemVer.Msbuild)](https://www.nuget.org/packages/NoeticTools.Git2SemVer.MSBuild)
[![Current Version](https://img.shields.io/nuget/v/NoeticTools.Git2SemVer.Tool?label=Git2SemVer.Tool)](https://www.nuget.org/packages/NoeticTools.Git2SemVer.Tool)
[![Release Build](https://github.com/NoeticTools/Git2SemVer/actions/workflows/dotnet.yml/badge.svg)](https://github.com/NoeticTools/Git2SemVer/actions/workflows/dotnet.yml)
<a href="https://github.com/NoeticTools/Git2SemVer">
  ![Static Badge](https://img.shields.io/badge/GitHub%20project-944248?logo=github)
</a>

# Git2SemVer

<div style="margin-left:0px; margin-top:-5px; margin-bottom:35px; font-family:Calibri; font-size:1.3em;">
No limits .NET solution versioning.</div>

Git2SemVer is a Visual Studio and developer friendly <a href="https://semver.org">Semantic Versioning</a> framework for .NET solution and project versioning.
It works the same with both Visual Studio and dotnet CLI builds. 
Every build, on both developer boxes and the build system, get traceable build numbering (no commit counting).

This tool is for teams that:

* Can benefit from true <a href="https://semver.org">Semantic Versioning</a>.
* Uses <a href="https://www.conventionalcommits.org/en/v1.0.0/">Conventional Commits</a> to automatically generate change logs.
* Uses branches to separate releasable code from feature or under development code (e.g: GitHub flow or GitFlow). 
* Only releases builds from a build system (or controlled host).
* Wants to avoid custom build scripts, and tools, on a build system.
* Uses Visual Studio as well as dotnet CLI.
* Values full traceability for every build regardless if on a build system or an uncontrolled developer box (commit counts/depth will not do).
* Needs unique versioning customisation that the built-in C# scripting may provide.

## Quick introduction

You:
* Mark a release's commit by adding a [git tag](xref:release-tagging) like `v1.2.3`.
* Use [separate branches](xref:branch-naming) for building release and non-release commits.
* Use <a href="https://www.conventionalcommits.org/en/v1.0.0/">Conventional Commit</a> mesages like `fix: fixed crash on shutdown`
to mark commits with fixes, features, and/or breaking changes.

Git2SemVer automatically, on every build, provides:
* Semantic versioning:
  * File version
  * Assembly version
  * Informational version
  * Package version (NuGet package version)
  * Other MSBuild version properties
  * [Pre-release identifier](xref:maturity-identifier) like `alpha`/`beta`/`rc` (from branch name)
* [Build number](#build-number)
* Host adaptive version formating like:
  * Including machine name in semantic version metadata when building on a developer's box.
  * Different build number sources and formating on GitHub Workflow and TeamCity.

Git2SemVer also detects and executes an optional [C# script](xref:csharp-script). This script can change any part of the versioning.

It can be configured for any mix of solution versioning and individual project versioning without external build-time tools.
No build system version generation steps are needed, keeps developer and build environments simple and aligned.

An example git workflow from a release `1.2.3` to the next release `2.0.0`:

```mermaid
gitGraph
        commit id:"1.2.3+100" tag:"1.2.3"
        branch feature/berry
        checkout feature/berry
        commit id:"1.2.3-beta.101"

        checkout main
        commit id:"1.2.3-alpha.102"
        checkout feature/berry

        branch develop/berry
        checkout develop/berry
        commit id:"feat:berry 1.3.0-alpha.103"
        checkout feature/berry
        merge develop/berry id:"1.3.0-beta.104"
        checkout main
        merge feature/berry id:"1.3.0+105"
        branch feature/peach
        checkout feature/peach
        commit id:"feat:peach 1.3.1-beta.106"
        commit id:"feat!:peach 2.0.0-beta.107"
        checkout main
        merge feature/peach id:"2.0.0+108" tag:"v2.0.0"
```

## Quick links

* [Getting Started](xref:getting-started)
* [Default Versioning](xref:versioning)
* Usage
  * [Workflow](xref:workflow)
  * [Release Tagging](xref:release-tagging)
  * [Branch naming](xref:branch-naming)
  * [Build Hosts](xref:build-hosts)
  * [C# Script](xref:csharp-script)


## Features

<br/>

<table style="margin-left:0px; margin-right:auto; align:left">

<!-- Conventional Commits -->
<tr>
    <td class="iconcolumn">
      <a href="https://www.conventionalcommits.org/en/v1.0.0/">
        <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/ConventionalCommits_128x128.png" height=64 width=64 />
      </a>
    </td>
    <td class="featureBody" style="vertical-align:center; text-align:left">
        <p class="featureTitle">
           <a href="https://www.conventionalcommits.org/en/v1.0.0/">Conventional Commits</a>
        </p>
        <p>
            Use conventional commits to automate both changelog generation and get semantic versioning for free.
        </p>
    </td>
</tr>

<!-- Visual studio -->
<tr>
    <td class="iconcolumn">
        <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/VisualStudio_128x128.png" height=64 width=64 />
    </td>
    <td class="featureBody" style="vertical-align:center; text-align:left">
        <p class="featureTitle">
            Visual Studio and .NET developer friendly
        </p>
        <p>
            Versioning on every Visual Studio or dotnet CLI build without exernal tools. Just build.
        </p>
    </td>
</tr>

<!-- Semver -->
<tr>
    <td class="iconcolumn">
      <a href="https://semver.org/">
        <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/SemVer122x64(dark).png" height=64 width=122 />
      </a>
    </td>
    <td class="featureBody" style="vertical-align:center; text-align:left">
        <p class="featureTitle">
          <a href="https://semver.org/">Semmantic Versioning</a>
        </p>
        <p>
           Industry standard Semmantic Version compliance.
        </p>
        <p>
           Supports <a href="https://noetictools.github.io/Git2SemVer.MSBuild/Reference/Glossary.html##initial-development">initial development versioning.</a> 
        </p>
</tr>

<!-- Environmental Parity -->
<tr>
    <td class="iconcolumn">
      <a href="https://noetictools.github.io/Git2SemVer.MSBuild/Reference/Glossary.html##environment-parity">
            <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/consistency_128x128.png"  height=64 width=64 />
      </a>
    </td>
    <td class="featureBody" style="vertical-align:center; text-align:left">
        <p class="featureTitle">
            <a href="https://noetictools.github.io/Git2SemVer.MSBuild/Reference/Glossary.html##environment-parity">Environment Parity</a>
        </p>
        <p>
            Visual Studio, VS Code, and dotnet CLI are all the same to Git2SemVer.
            Build system and developer environments all get versioning without custom build steps, tools, or scripts.
        </p>
    </td>
</tr>

<!-- Build Numbering -->
<tr>
    <td class="iconcolumn">
        <p style="font-size:50px; margin:0px;color:DarkCyan;">#</p>
    </td>
    <td class="featureBody" style="vertical-align:center; text-align:left">
        <p class="featureTitle">
          <a href="https://noetictools.github.io/Git2SemVer.MSBuild/Reference/Glossary.html##build-number">
            Build Numbering
          </a>
        </p>
        <p>
            Automatic build numbering on all <b>developer boxes</b> and build system builds.
        <p>
        <p>
           Full traceability.
           <a href="https://noetictools.github.io/Git2SemVer.MSBuild/Reference/Glossary.html##build-height">Build height</a> <b>NOT</b> used.
        </p>
    </td>
</tr>

<!-- C# -->
<tr>
    <td class="iconcolumn">
      <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/CSharp_128x128.png" height=64 width=64 />
    </td>
    <td class="featureBody" style="vertical-align:center; text-align:left">
        <p class="featureTitle">
           <a href="https://noetictools.github.io/Git2SemVer.MSBuild/Usage/CSharpScripting/CSharpScript.html">C# Scripting</a>
        </p>
        <p>
           <b>No limits customisable</b> by built-in C# scripting with a versioning API.
        </p>
    </td>
</tr>

<!-- Build Host Adaptive Versioning -->
<tr>
    <td class="iconcolumn">
      <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/ComputerMonitor.png" height=64 width=64 />
    </td>
    <td class="featureBody" style="vertical-align:center; text-align:left">
        <p class="featureTitle">
           Build Host Adaptive Versioning
        </p>
        <p>The versioning adapts according to the build host.</p>
        <p>e.g: Automatically uses TeamCity build counter for build number and drops machine name metadata.</p>
    </td>
</tr>

<!-- Workflow agnostic -->
<tr>
    <td class="iconcolumn">
      <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/git_workflow_128x128.png" height=64 width=64 />
    </td>
    <td class="featureBody" style="vertical-align:center; text-align:left">
        <p class="featureTitle">Git Workflow Agnostic</p>
        <p>No Git workflow configuration required. It works the same for GitFlow and GitHub Flow.</p>
    </td>
</tr>

<!-- TeamCity -->
<tr>
    <td class="iconcolumn">
      <a href="https://noetictools.github.io/Git2SemVer.MSBuild/Usage/BuildHosts/TeamCity.html">
        <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/TeamCity_128x128.png" height=64 width=64 />
      </a>
    </td>
    <td class="featureBody" style="vertical-align:center; text-align:left">
        <p class="featureTitle">
           <a href="https://noetictools.github.io/Git2SemVer.MSBuild/Usage/BuildHosts/TeamCity.html">
            TeamCity Integration
           </a>
        </p>
        <p>
           Automatic build system detection with server build number (label) updated with
           a build version specifically adapted for TeamCity.
         </p>
         <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/TeamCity-01.png">
    </td>
</tr>

<!-- GitHub Workflows -->
<tr>
    <td class="iconcolumn">
      <a href="https://noetictools.github.io/Git2SemVer.MSBuild/Usage/BuildHosts/GitHubWorkflows.html">
        <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/github_gray_128x128.png" height=64 width=64 />
      </a>
    </td>
    <td class="featureBody" style="vertical-align:center; text-align:left">
        <p class="featureTitle">
          <a href="https://noetictools.github.io/Git2SemVer.MSBuild/Usage/BuildHosts/GitHubWorkflows.html">
            GitHub Workflows
          </a>
        </p>
        <p>Built in support for builds using GitHub Actions.</p>
        <p>Constructs build number from run and run attempt numbers
        and adapts the version for GitHub builds.</p>
        </p>
    </td>
</tr>

</table>

<br/>

 
## License

Git2SemVer uses the [MIT license](https://choosealicense.com/licenses/mit/).


## Acknowledgments

This project uses the following tools and libraries. Many thanks to those who created and manage them.

* [Spectre.Console](https://github.com/spectreconsole/spectre.console)
* [Injectio](https://github.com/loresoft/Injectio)
* [JetBrains Annotations](https://www.jetbrains.com/help/resharper/Code_Analysis__Code_Annotations.html)
* [TeamCity.ServiceMessages](https://github.com/JetBrains/TeamCity.ServiceMessages)
* [Semver](https://www.nuget.org/packages/Semver) - files copied to create subset
* [NuGet.Versioning](https://www.nuget.org/packages/NuGet.Versioning)
* [NUnit](https://www.nuget.org/packages/NUnit)
* [Moq](https://github.com/devlooped/moq)
* [docfx](https://dotnet.github.io/docfx/)
* [JsonPeek](https://www.clarius.org/json/)
* <a href="https://www.flaticon.com/free-icons/brain" title="brain icons">Brain icons created by Freepik - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/consistent" title="consistent icons">Consistent icons created by Freepik - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/programmer" title="programmer icons">Programmer icons created by Flowicon - Flaticon</a>
