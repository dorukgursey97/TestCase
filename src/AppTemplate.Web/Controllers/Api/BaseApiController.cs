using AppTemplate.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace AppTemplate.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult ToResponse<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Value);

        if (result.Errors.Any())
            return BadRequest(new { errors = result.Errors });

        return result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false
            ? NotFound(new { error = result.Error })
            : BadRequest(new { error = result.Error });
    }

    protected IActionResult ToResponse(Result result)
    {
        if (result.IsSuccess)
            return NoContent();

        if (result.Errors.Any())
            return BadRequest(new { errors = result.Errors });

        return result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false
            ? NotFound(new { error = result.Error })
            : BadRequest(new { error = result.Error });
    }
}
