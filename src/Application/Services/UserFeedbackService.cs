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
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    /// <summary>
    /// Service for managing userFeedback-related operations.
    /// </summary>
    public class UserFeedbackService : IUserFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserFeedbackService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations.</param>
        /// <param name="mapper">The mapper for object mapping.</param>
        public UserFeedbackService(IUnitOfWork unitOfWork, IMapper mapper)
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
        public async Task<Result<CreatedResponseDto>> AddAsync(UserFeedbackAddRequestDto addRequestDto)
        {
            var userFeedback = _mapper.Map<UserFeedback>(addRequestDto);
            await _unitOfWork.UserFeedbackRepository.AddAsync(userFeedback);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new CreatedResponseDto (userFeedback.Id));
        }

        /// <inheritdoc/>
        public async Task<Result<SuccessResponseDto>> DeleteAsync(long id)
        {
            var exists = await _unitOfWork.UserFeedbackRepository.ExistsAsync(id);
            if (!exists)
            {
                return Result.Fail<SuccessResponseDto>("User feedback not found.");
            }

            await _unitOfWork.UserFeedbackRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new SuccessResponseDto { Message = "User feedback deleted successfully." });
        }

        /// <inheritdoc/>
        public async Task<Result<PaginatedList<UserFeedbackDto>>> GetAsync(QueryFilterDto queryFilter)
        {
            var paginatedList = await _unitOfWork.UserFeedbackRepository.GetAsync(queryFilter);

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
            var userFeedback = await _unitOfWork.UserFeedbackRepository.GetByIdAsync(id);
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
            var userFeedback = await _unitOfWork.UserFeedbackRepository.GetByIncidentIdAsync(incident);
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
            var userFeedback = await _unitOfWork.UserFeedbackRepository.GetByIdAsync(id);
            if (userFeedback == null)
            {
                return Result.Fail<SuccessResponseDto>("User feedback not found.");
            }

            _mapper.Map(updateRequestDto, userFeedback);
            _unitOfWork.UserFeedbackRepository.Update(userFeedback);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new SuccessResponseDto { Message = "User feedback updated successfully." });
        }
    }
}

