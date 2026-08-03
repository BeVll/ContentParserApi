using ContentParserApi.Constants;

namespace ContentParserApi.Models.Requests;

public class ParseContentRequest
{
    public ContentType Type { get; set; }
    public string Content { get; set; } = string.Empty;
}