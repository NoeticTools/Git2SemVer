namespace NoeticTools.Git2SemVer.Tool.CommandLine;

internal enum ReturnCodes
{
    Succeeded = 0,
    CommandLineParsingError = 1,
    CommandError = 2,
    NoCommandError = 3,
    UnknownError = 5
}