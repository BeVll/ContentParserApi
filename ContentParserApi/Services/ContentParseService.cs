using System.Globalization;
using System.Text;
using System.Text.Json;
using ContentParserApi.Constants;
using ContentParserApi.Interfaces;
using ContentParserApi.Models.Responses;
using CsvHelper;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ContentParserApi.Services;

public class ContentParseService : IContentParseService
{
    public ParseContentResponse Parse(ContentType type, string content)
    {
        string decodedContent;

        try
        {
            var bytes = Convert.FromBase64String(content);
            decodedContent = Encoding.UTF8.GetString(bytes);
        }
        catch (FormatException )
        {
            return Error("Invalid Base64 content");
        }

        try
        {
            var data = type switch
            {
                ContentType.CSV => ParseCsv(decodedContent), 
                ContentType.INTERNAL_JSON => ParseInternalJson(decodedContent),
                _ => throw new ArgumentOutOfRangeException(nameof(type), "Invalid content type")
            };

            return new ParseContentResponse
            {
                Status = ResponseStatus.Success,
                Count = data.Count,
                Data = data
            };
        }
        catch (Exception ex)
        {
            return Error(ex.Message);
        }
    }

    private static IReadOnlyList<Dictionary<string, string>> ParseCsv(string content)
    {
        using var reader = new StringReader(content);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Read();
        csv.ReadHeader();

        var rows = new List<Dictionary<string, string>>();

        while (csv.Read())
        {
            var row = new Dictionary<string, string>();

            foreach (var header in csv.HeaderRecord!)
            {
                row[header] = csv.GetField(header) ?? string.Empty;
            }
                
            rows.Add(row);
        }

        return rows;
    }

    private static IReadOnlyList<Dictionary<string, string>> ParseInternalJson(string content)
    {
        List<Dictionary<string, JsonElement>>? rawItems;
        try
        {
            rawItems = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(content);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("INTERNAL_JSON content is invalid", ex);
        }

        if (rawItems is null)
            throw new InvalidOperationException("INTERNAL_JSON content is invalid");

        if (rawItems.Count == 0)
        {
            throw new InvalidOperationException("INTERNAL_JSON must contain at least one object");
        }

        var result = new List<Dictionary<string, string>>(rawItems.Count);

        foreach (var item in rawItems)
        {
            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var (key, value) in item)
            {
                row[key] = value.ValueKind switch
                {
                    JsonValueKind.String => value.GetString() ?? string.Empty,
                    JsonValueKind.Number => value.ToString(),
                    JsonValueKind.True => "true",
                    JsonValueKind.False => "false",
                    JsonValueKind.Null => string.Empty,
                    _ => throw new InvalidOperationException(
                        $"Unsupported JSON value for field '{key}'.")
                };
            }
            
            result.Add(row);
        }

        return result;
    }

    private static ParseContentResponse Error(string message) => new()
    {
        Status = ResponseStatus.Error,
        Count = 0,
        Data = [],
        Error = message
    };
}