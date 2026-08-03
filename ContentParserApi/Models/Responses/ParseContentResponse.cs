using ContentParserApi.Constants;

namespace ContentParserApi.Models.Responses;

public class ParseContentResponse
{
    public ResponseStatus Status { get; init; } = ResponseStatus.Success;
    public int Count { get; init; }
    public IReadOnlyList<Dictionary<string, string>> Data { get; init; } = [];
    public string? Error { get; init; }
}