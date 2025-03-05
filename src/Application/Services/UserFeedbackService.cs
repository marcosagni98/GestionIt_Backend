using Application.Dtos.CommonDtos;
using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.UserFeedbacks;
using Application.Dtos.CRUD.UserFeedbacks.Request;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using FluentResults;

namespace Application.Services
{
    /// <summary>
    /// Service for managing userFeedback-related operations.
    /// </summary>
    public class UserFeedbackService : IUserFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserFeedbackRepository _userFeedbackRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserFeedbackService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations.</param>
        /// <param name="mapper">The mapper for object mapping.</param>
        public UserFeedbackService(IUnitOfWork unitOfWork, IMapper mapper, IUserFeedbackRepository userFeedbackRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userFeedbackRepository = userFeedbackRepository;
        }

        /// <inheritdoc/>
        public async Task<Result<CreatedResponseDto>> AddAsync(UserFeedbackAddRequestDto addRequestDto)
        {
            var userFeedback = _mapper.Map<UserFeedback>(addRequestDto);
            await _userFeedbackRepository.AddAsync(userFeedback);
            await _unitOfWork.SaveAsync();

            return Result.Ok(new CreatedResponseDto(userFeedback.Id));
        }

        /// <inheritdoc/>
        public async Task<Result<SuccessResponseDto>> DeleteAsync(long id)
        {
            var exists = await _userFeedbackRepository.ExistsAsync(id);
            if (!exists)
            {
                return Result.Fail<SuccessResponseDto>("User feedback not found.");
            }

            await _userFeedbackRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();

            return Result.Ok(new SuccessResponseDto { Message = "User feedback deleted successfully." });
        }

        /// <inheritdoc/>
        public async Task<Result<PaginatedList<UserFeedbackDto>>> GetAsync(QueryFilterDto queryFilter)
        {
            var paginatedList = await _userFeedbackRepository.GetAsync(queryFilter);

            if (paginatedList == null || paginatedList.Items == null)
            {
                return Result.Fail<PaginatedList<UserFeedbackDto>>("Error retrieving user feedbacks.");
            }

            var userFeedbackDtos = _mapper.Map<List<UserFeedbackDto>>(paginatedList.Items);

            return Result.Ok(new PaginatedList<UserFeedbackDto>(userFeedbackDtos, paginatedList.TotalCount));
        }

        /// <inheritdoc/>
        public async Task<Result<UserFeedbackDto>> GetByIdAsync(long id)
        {
            var userFeedback = await _userFeedbackRepository.GetByIdAsync(id);
            if (userFeedback == null)
            {
                return Result.Fail<UserFeedbackDto>("User feedback not found.");
            }

            var response = _mapper.Map<UserFeedbackDto>(userFeedback);
            return Result.Ok(response);
        }

        /// <inheritdoc/>
        public async Task<Result<UserFeedbackDto>> GetByIncidentIdAsync(long incident)
        {
            var userFeedback = await _userFeedbackRepository.GetByIncidentIdAsync(incident);
            if (userFeedback == null)
            {
                return Result.Fail<UserFeedbackDto>("User feedback not found.");
            }

            var response = _mapper.Map<UserFeedbackDto>(userFeedback);
            return Result.Ok(response);
        }

        /// <inheritdoc/>
        public async Task<Result<SuccessResponseDto>> UpdateAsync(long id, UserFeedbackUpdateRequestDto updateRequestDto)
        {
            var userFeedback = await _userFeedbackRepository.GetByIdAsync(id);
            if (userFeedback == null)
            {
                return Result.Fail<SuccessResponseDto>("User feedback not found.");
            }

            _mapper.Map(updateRequestDto, userFeedback);
            _userFeedbackRepository.Update(userFeedback);
            await _unitOfWork.SaveAsync();

            return Result.Ok(new SuccessResponseDto { Message = "User feedback updated successfully." });
        }
    }
}

