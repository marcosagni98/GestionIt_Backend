using Application.Dtos.CommonDtos;
using Application.Dtos.CRUD.Messages;
using Application.Dtos.CRUD.Messages.Request;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using FluentResults;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Services
{

    /// <summary>
    /// Service for managing message-related operations.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="MessageService"/> class.
    /// </remarks>
    /// <param name="logger">Logger interface</param>
    /// <param name="unitOfWork">The unit of work for database operations.</param>
    /// <param name="mapper">The mapper for object mapping.</param>
    /// <param name="incidentRepository">Incident repository</param>
    /// <param name="messageRepository">Message repository</param>
    public class MessageService(ILogger<MessageService> logger, IUnitOfWork unitOfWork, IMapper mapper, IMessageRepository messageRepository, IIncidentRepository incidentRepository) : IMessageService
    {
        private readonly ILogger<MessageService> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IMessageRepository _messageRepository = messageRepository;
        private readonly IIncidentRepository _incidentRepository = incidentRepository;

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
            if(incident == null)
{
                string error = $"Incident with id {incidentId} not found";
                _logger.LogError(error);
                return Result.Fail(error);
            }

            List<MessageDto> messages = _mapper.Map<List<MessageDto>>(await _messageRepository.GetByIncidentIdAsync(incidentId));
            
            return Result.Ok(messages);
        }
    }
}
