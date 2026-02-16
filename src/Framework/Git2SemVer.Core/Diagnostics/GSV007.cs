// ReSharper disable UnusedType.Global
// ReSharper disable InconsistentNaming

namespace NoeticTools.Git2SemVer.Core.Diagnostics;

[DiagnosticCode]
public sealed class GSV007 : DiagnosticCodeBase
{
    public GSV007()
        : base(7,
               "Versioning",
               "This occurs when build property `Git2SemVer_ScriptPath`, `Git2SemVer_ScriptArg`, or `Git2SemVer_RunScript` is set.",
               """
               The properties `Git2SemVer_ScriptPath`, `Git2SemVer_ScriptArg`, and `Git2SemVer_RunScript` are no longer used since C# scripting was removed in version 4.0.0. 
               Remove these properties from the build.
               """,
               "The script file path build properties Git2SemVer_ScriptPath, Git2SemVer_ScriptArg, and Git2SemVer_RunScript are obsolete.")
    {
    }
}