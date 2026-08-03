using ContentParserApi.Constants;
using ContentParserApi.Models.Requests;
using ContentParserApi.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ContentParserApi.Controllers;

[ApiController]
[Route("api/v1")]
public sealed class ParseContentController : Controller
{
    
    [HttpPost("parse-content")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public ActionResult<ParseContentResponse> ParseContent([FromBody] ParseContentRequest request)
    {
        if (string.IsNullOrEmpty(request.Content))
        {
            return BadRequest(new ParseContentResponse()
            {
                Status = ResponseStatus.Error,
                Count = 0,
                Data = [],
                Error = "Content is required"
            });
        }

        if (!Enum.IsDefined(typeof(ContentType), request.Type))
        {
            return BadRequest(new ParseContentResponse()
            {
                Status = ResponseStatus.Error,
                Count = 0,
                Data = [],
                Error = "Unsupported content type"
            });
        }

        return Ok();
    }
}