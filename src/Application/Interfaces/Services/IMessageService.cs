using Application.Dtos.CRUD.Messages;
using Application.Dtos.CRUD.Messages.Request;

namespace Application.Interfaces.Services;

/// <summary>
/// Message Service Interface
/// </summary>
public interface IMessageService : IBaseService<MessageDto, MessageAddRequestDto, MessageUpdateRequestDto>, IDisposable
{
    Task HandleMessageAsync(string user, string message);
}
