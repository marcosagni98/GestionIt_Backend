using Application.Dtos.CRUD.Messages.Request;
using Application.Interfaces.Services;
using Azure.Identity;
using FluentResults;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

/// <summary>
/// Hub for managing incident-based chat groups and message distribution.
/// </summary>
/// <remarks>
/// Initializes ChatHub with required services.
/// </remarks>
/// <param name="logger">The logger interface</param>
/// <param name="messageService">Service for message operations.</param>
/// <param name="incidentService">Service for incident data.</param>
/// <param name="userService">Service for user verification.</param>
public class ChatHub(ILogger<ChatHub> logger, IMessageService messageService, IIncidentService incidentService, IUserService userService) : Hub
{
    private readonly ILogger<ChatHub> _logger = logger;
    private readonly IMessageService _messageService = messageService;
    private readonly IIncidentService _incidentService = incidentService;
    private readonly IUserService _userService = userService;

    /// <summary>
    /// Adds the connected user to incident groups on connection.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        long userId = await GetUserIdAsync();
        List<long> incidentsIds = await GetIncidetIdsByUserIdAsync(userId);

        foreach (var incidentId in incidentsIds)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, incidentId.ToString());
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Removes the user from all incident groups on disconnection.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        long userId = await GetUserIdAsync();
        List<long> incidentsIds = await GetIncidetIdsByUserIdAsync(userId);

        foreach (var incidentId in incidentsIds)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, incidentId.ToString());
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Sends a message to the specified incident group and saves it.
    /// </summary>
    /// <param name="incidentId">ID of the incident group.</param>
    /// <param name="userId">ID of the sender.</param>
    /// <param name="text">Content of the message.</param>
    public async Task SendMessage(long incidentId, long userId, string name, string text)
    {
        MessageAddRequestDto message = new(
            IncidentId: incidentId,
            SenderId: userId,
            SenderName: name,
            Text: text,
            SentAt: DateTime.UtcNow
        );

        await _messageService.AddAsync(message);
        await Clients.Group(incidentId.ToString()).SendAsync("ReceiveMessage", message);
    }

    #region private methods

    /// <summary>
    /// Gets incident IDs the user is associated with.
    /// </summary>
    private async Task<List<long>> GetIncidetIdsByUserIdAsync(long userId)
    {
        Result<List<long>> incidentIdsResult = await _incidentService.GetIncidentIdsByUserIdAsync(userId);
        return incidentIdsResult.IsFailed ? new List<long>() : incidentIdsResult.Value;
    }

    /// <summary>
    /// Retrieves and verifies the user ID from the connection context.
    /// </summary>
    private async Task<long> GetUserIdAsync()
    {
        string? userId = Context.GetHttpContext().Request.Query["userId"];
        Result<long> userResult = await _userService.VerifyUserAsync(userId);
        return userResult.IsFailed ? 0 : userResult.Value;
    }

    #endregion
}

