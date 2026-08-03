using ContentParserApi.Constants;
using ContentParserApi.Models.Responses;

namespace ContentParserApi.Interfaces;

public interface IContentParseService
{
    ParseContentResponse Parse(ContentType type, string content);
}