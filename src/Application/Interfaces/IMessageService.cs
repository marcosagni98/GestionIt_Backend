using Application.Dtos.CRUD.Messages;
using Application.Dtos.CRUD.Messages.Request;
using Application.Dtos.CRUD.Messages.Response;

namespace Application.Interfaces;

public interface IMessageService : IBaseService<MessageDto, MessageResponseDto, MessageAddRequestDto, MessageUpdateRequestDto>, IDisposable
{
}
