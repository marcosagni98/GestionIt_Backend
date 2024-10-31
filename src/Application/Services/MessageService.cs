using Application.Dtos.CommonDtos;
using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.Messages;
using Application.Dtos.CRUD.Messages.Request;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{

    /// <summary>
    /// Service for managing message-related operations.
    /// </summary>
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations.</param>
        /// <param name="mapper">The mapper for object mapping.</param>
        public MessageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Dispose
        private bool disposed = false;

        /// <summary>
        /// Releases the unmanaged resources used by the service and optionally releases
        /// the managed resources if disposing is true.
        /// </summary>
        /// <param name="disposing">Indicates whether the method was called directly
        /// or from a finalizer. If true, the method has been called directly
        /// and managed resources should be disposed. If false, it was called by the
        /// runtime from inside the finalizer and only unmanaged resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _unitOfWork.Dispose();
                }
            }
            disposed = true;
        }

        /// <summary>
        /// Releases all resources used by the service.
        /// This method is called by consumers of the service when they are done
        /// using it to free resources promptly.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <inheritdoc/>
        public async Task<Result<CreatedResponseDto>> AddAsync(MessageAddRequestDto addRequestDto)
        {
            var message = _mapper.Map<Message>(addRequestDto); // Asegúrate de que el DTO y la entidad coincidan
            await _unitOfWork.MessageRepository.AddAsync(message);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new CreatedResponseDto (message.Id));
        }

        /// <inheritdoc/>
        public async Task<Result<SuccessResponseDto>> DeleteAsync(long id)
        {
            var exists = await _unitOfWork.MessageRepository.ExistsAsync(id);
            if (!exists)
            {
                return Result.Fail<SuccessResponseDto>("Message not found.");
            }

            await _unitOfWork.MessageRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new SuccessResponseDto { Message = "Message deleted successfully." });
        }

        /// <inheritdoc/>
        public async Task<Result<PaginatedList<MessageDto>>> GetAsync(QueryFilterDto queryFilter)
        {
            var paginatedList = await _unitOfWork.MessageRepository.GetAsync(queryFilter);

            if (paginatedList == null || paginatedList.Items == null)
            {
                return Result.Fail<PaginatedList<MessageDto>>("Error retrieving messages.");
            }

            var messageDtos = _mapper.Map<List<MessageDto>>(paginatedList.Items);

            return Result.Ok(new PaginatedList<MessageDto>(messageDtos, paginatedList.TotalCount));
        }

        /// <inheritdoc/>
        public async Task<Result<MessageDto>> GetByIdAsync(long id)
        {
            var message = await _unitOfWork.MessageRepository.GetByIdAsync(id);
            if (message == null)
            {
                return Result.Fail<MessageDto>("Message not found.");
            }

            var response = _mapper.Map<MessageDto>(message);
            return Result.Ok(response);
        }

        /// <inheritdoc/>
        public async Task<Result<SuccessResponseDto>> UpdateAsync(long id, MessageUpdateRequestDto updateRequestDto)
        {
            var message = await _unitOfWork.MessageRepository.GetByIdAsync(id);
            if (message == null)
            {
                return Result.Fail<SuccessResponseDto>("Message not found.");
            }

            _mapper.Map(updateRequestDto, message);
            await _unitOfWork.MessageRepository.UpdateAsync(message);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new SuccessResponseDto { Message = "Message updated successfully." });
        }

        /// <inheritdoc/>
        public Task HandleMessageAsync(string user, string message)
        {
            // Aquí puede ir la lógica de negocio, como almacenar el mensaje o aplicar validaciones.
            return Task.CompletedTask;
        }
    }
}
