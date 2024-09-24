using Application.Dtos.CommonDtos.Response;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers.v1;

[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status200OK,Type =typeof(SuccessResponseDto))]
[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetails))]
[ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorDetails))]
[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
[Route("api/v1/[controller]")]
[ApiController]
public class BaseApiController : ControllerBase
{
   
}
