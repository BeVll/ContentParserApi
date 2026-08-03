using ContentParserApi.Constants;

namespace ContentParserApi.Models.Requests;

public class ParseContentRequest
{
    public string Type { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
}