using System.Globalization;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using ContentParserApi.Constants;
using ContentParserApi.Interfaces;
using ContentParserApi.Models.Responses;
using CsvHelper;

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
                ContentType.INTERNAL_JSON => ParseCsv(decodedContent),
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
        using (var reader = new StreamReader(content))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords<Dictionary<string, string>>();

            return records.ToList();
        }
        
    }

    private static ParseContentResponse Error(string message) => new()
    {
        Status = ResponseStatus.Error,
        Count = 0,
        Data = [],
        Error = message
    };
}