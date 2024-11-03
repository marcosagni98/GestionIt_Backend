using Application.Dtos.CommonDtos;
using Application.Dtos.CRUD.Messages;
using Application.Dtos.CRUD.Messages.Request;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using FluentResults;

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

            return Result.Ok(new CreatedResponseDto(message.Id));
        }

        /// <inheritdoc/>
        public async Task<Result<List<MessageDto>>> GetByIncidentIdAsync(long incidentId)
        {
            Incident? incident = await _unitOfWork.IncidentRepository.GetByIdAsync(incidentId);
            if(incident == null) return Result.Fail<List<MessageDto>>("Incident not found.");

            List<MessageDto> messages = _mapper.Map<List<MessageDto>>(await _unitOfWork.MessageRepository.GetByIncidentIdAsync(incidentId));
            
            return Result.Ok(messages);
        }
    }
}
