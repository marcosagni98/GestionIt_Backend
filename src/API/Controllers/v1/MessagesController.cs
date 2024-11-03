using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.Messages;
using Application.Dtos.CRUD.Messages.Request;
using Application.Interfaces.Services;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1;

/// <summary>
/// Controller for managing message-related operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MessageController"/> class.
/// </remarks>
/// <param name="messageService">The message service.</param>
[Produces("application/json")]
[Route("api/v1/[controller]")]
public class MessageController(IMessageService messageService) : BaseApiController
{
    private readonly IMessageService _messageService = messageService;
}
