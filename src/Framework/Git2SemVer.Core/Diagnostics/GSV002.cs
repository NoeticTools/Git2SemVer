// ReSharper disable InconsistentNaming

namespace NoeticTools.Git2SemVer.Core.Diagnostics;

[DiagnosticCode]
[Obsolete("This diagnostic code is obsolete since C# scripting was removed in version 4.0.0.", true)]
public sealed class GSV002 : DiagnosticCodeBase
{
    public GSV002()
        : base(2,
               "Versioning",
               "This occurs when the build property `Git2SemVer_ScriptPath` is null or whitespaces and the property `Git2SemVer_RunScript` is not `false`.",
               """
               This only applies version prior to 4.0.0 when C# scripting was removed. 
               
               If there is a C# script to run, set the property to script's path build property `Git2SemVer_ScriptPath`.

               Otherwise set `Git2SemVer_RunScript` to `false` by add the following
               to the solution's `Directory.Build.props` (if it exists) or the project's file.

               ```xml
               <PropertyGroup>
                 <Git2SemVer_RunScript>false</Git2SemVer_RunScript>
               </PropertyGroup>
               ```
               """,
               "The script file path build property BuildScriptPath is required.")
    {
    }
}