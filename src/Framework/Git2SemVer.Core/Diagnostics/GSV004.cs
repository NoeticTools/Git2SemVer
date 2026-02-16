// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming

namespace NoeticTools.Git2SemVer.Core.Diagnostics;

[DiagnosticCode]
[Obsolete("This diagnostic code is obsolete since C# scripting was removed in version 4.0.0.", true)]
public sealed class GSV004 : DiagnosticCodeBase
{
    public GSV004(string buildScriptPath)
        : base(4,
               "Versioning",
               "This occurs when the build property `Git2SemVer_ScriptPath` (`{0}`) is not a valid path and `RunScript` is `true`.",
               """
               This only applies version prior to 4.0.0 when C# scripting was removed. 
               
               If there is a C# script to run, correct the `Git2SemVer_ScriptPath` property to script's path.

               Otherwise set RunScript to `false` by add the following
               to the solution's `Directory.Build.props` (if it exists) or the project's file.

               ```xml
               <PropertyGroup>
                 <Git2SemVer_RunScript>false</Git2SemVer_RunScript>
               </PropertyGroup>
               ```
               """,
               "The script file not found and is required.",
               buildScriptPath)
    {
    }
}