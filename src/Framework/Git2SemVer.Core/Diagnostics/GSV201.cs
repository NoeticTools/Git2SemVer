// ReSharper disable UnusedType.Global
// ReSharper disable InconsistentNaming

namespace NoeticTools.Git2SemVer.Core.Diagnostics;

[DiagnosticCode]
public sealed class GSV201 : DiagnosticCodeBase
{
    public GSV201(string dataDirectory, string changelogFileName)
        : base(201,
               subcategory: "Changelog",
               description: """
                            The occurs when the changelog's last run data file cannot be loaded. 
                            The given data directory ({0}) may be incorrect or the last run data file ({1}.data.g.json) may not be added to the repository.
                            """,
               resolution: """
                           If this is the first time the generator has been run then it is safe to ignore.

                           Otherwise, check that the last run data file ({1}.data.g.json) exists. If it does, open it in an editor to see if the JSON formating is corrupted.
                           If it does not exist add the last run data file ({1}.data.g.json), that the generator has created, to the repository

                           Ensure that the data directory option used is correct.
                           ```
                           """,
               message: "The last run data file ({1}.data.g.json) must exist in the data directory '{0}'.",
               dataDirectory,
               changelogFileName)
    {
    }
}