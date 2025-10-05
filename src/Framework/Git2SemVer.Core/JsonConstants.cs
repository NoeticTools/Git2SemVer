using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;


namespace NoeticTools.Git2SemVer.Core;

public static class JsonConstants
{
    public static readonly TimeSpan ReadTimeLimit = TimeSpan.FromSeconds(10);

    public static readonly JsonSerializerOptions SerialiseOptions = new()
    {
        WriteIndented = true,
        IgnoreReadOnlyFields = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public static readonly TimeSpan WriteTimeLimit = TimeSpan.FromSeconds(10);
}