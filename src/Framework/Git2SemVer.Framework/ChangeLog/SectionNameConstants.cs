namespace NoeticTools.Git2SemVer.Framework.ChangeLog;

internal static class SectionNameConstants
{
    /// <summary>
    ///     Name of the section in the generated Markdown document that contains the current or next release changes.
    /// </summary>
    public static string NextRelease => "next release";

    /// <summary>
    ///     Name of the section in the generated Markdown document where newly found changes are placed.
    /// </summary>
    public static string UngroomedChangesSuffix => " - changes";

    /// <summary>
    ///     Name of the section in the generated Markdown document that contains the current or next release version or
    ///     'Unreleased'.
    /// </summary>
    public static string Version => "version";
}