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
        private readonly IMessageRepository _messageRepository;
        private readonly IIncidentRepository _incidentRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations.</param>
        /// <param name="mapper">The mapper for object mapping.</param>
        public MessageService(IUnitOfWork unitOfWork, IMapper mapper, IMessageRepository messageRepository, IIncidentRepository incidentRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _messageRepository = messageRepository;
            _incidentRepository = incidentRepository;
        }

        /// <inheritdoc/>
        public async Task<Result<CreatedResponseDto>> AddAsync(MessageAddRequestDto addRequestDto)
        {
            var message = _mapper.Map<Message>(addRequestDto);
            await _messageRepository.AddAsync(message);
            await _unitOfWork.SaveAsync();

            return Result.Ok(new CreatedResponseDto(message.Id));
        }

        /// <inheritdoc/>
        public async Task<Result<List<MessageDto>>> GetByIncidentIdAsync(long incidentId)
        {
            Incident? incident = await _incidentRepository.GetByIdAsync(incidentId);
            if (incident == null) return Result.Fail<List<MessageDto>>("Incident not found.");

            List<MessageDto> messages = _mapper.Map<List<MessageDto>>(await _messageRepository.GetByIncidentIdAsync(incidentId));

            return Result.Ok(messages);
        }
    }
}
