using Microsoft.AspNetCore.Mvc;
using Set.Auth.Api.Models.Response;
using System.Net;

namespace Set.Auth.Api.Controllers.Base;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    protected IActionResult InternalServerError(Exception ex)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseResult<object>.FailureResponse(ex.Message));
    }

    protected ResponseResult ErrorMessage(HttpStatusCode statusCode, string message, int errorCode = -1)
    {
        return ResponseResult.FailureResponse(message, new Dictionary<string, string[]>
        {
            { "ErrorCode", new[] { errorCode.ToString() } },
            { "StatusCode", new[] { ((int)statusCode).ToString() } }
        });
    }

    protected ResponseResult ErrorMessage(string message, int errorCode = -1, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return ResponseResult.FailureResponse(message, new Dictionary<string, string[]>
        {
            { "errorCode", new[] { errorCode.ToString() } },
            { "statusCode", new[] { ((int)statusCode).ToString() } }
        });
    }

    protected ResponseResult<T> SuccessData<T>(T data) where T : class
    {
        return ResponseResult<T>.SuccessResponse(data);
    }

    protected ResponseResult<T> SuccessData<T>(T data, string message) where T : class
    {
        return ResponseResult<T>.SuccessResponse(data, message);
    }

    protected ResponseResult SuccessMessage(string message)
    {
        return ResponseResult.SuccessResponse(message);
    }
}
