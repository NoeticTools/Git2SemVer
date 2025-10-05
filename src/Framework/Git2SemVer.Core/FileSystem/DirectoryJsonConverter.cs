using System.Text.Json;
using System.Text.Json.Serialization;
using NoeticTools.Git2SemVer.Core.Exceptions;


namespace NoeticTools.Git2SemVer.Core.FileSystem;

public class DirectoryJsonConverter : JsonConverter<Directory>
{
    public override Directory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Git2SemVerArgumentException.ThrowIfNull(options, nameof(options));

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected a string value for Directory path.");
        }

        var path = reader.GetString();
        if (path == null)
        {
            throw new Git2SemVerInvalidFormatException("Expected non-null string for Directory path.");
        }

        return new Directory(path);
    }

    public override void Write(Utf8JsonWriter writer, Directory directory, JsonSerializerOptions options)
    {
        Git2SemVerArgumentException.ThrowIfNull(writer, nameof(writer));
        Git2SemVerArgumentException.ThrowIfNull(directory, nameof(directory));
        Git2SemVerArgumentException.ThrowIfNull(options, nameof(options));
        writer.WriteStringValue(directory!.ToString());
    }
}