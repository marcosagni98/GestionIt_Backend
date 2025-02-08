using Application.Dtos.CommonDtos;
using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.Users;
using Application.Dtos.CRUD.Users.Request;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using FluentResults;

namespace Application.Services
{
    /// <summary>
    /// Service for managing user-related operations.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations.</param>
        /// <param name="mapper">The mapper for object mapping.</param>

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        /// <inheritdoc/>
        public async Task<Result<CreatedResponseDto>> AddAsync(UserAddRequestDto addRequestDto)
        {
            var user = _mapper.Map<User>(addRequestDto);
            if(await _userRepository.CountAsync() == 0)
            {
                user.UserType = UserType.Admin;
            }
            else 
            { 
                var result = await _userRepository.EmailExistsAsync(user.Email);
                if (result)
                {
                    return Result.Fail<CreatedResponseDto>("Email already exists.");
                }
            }
            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveAsync();

            return Result.Ok(new CreatedResponseDto(user.Id));
        }

        /// <inheritdoc/>
        public async Task<Result<SuccessResponseDto>> UpdateAsync(long id, UserUpdateRequestDto updateRequestDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return Result.Fail<SuccessResponseDto>("User not found.");
            }

            _mapper.Map(updateRequestDto, user);
            _userRepository.Update(user);
            await _unitOfWork.SaveAsync();

            return Result.Ok(new SuccessResponseDto { Message = "User updated successfully." });
        }

        /// <inheritdoc/>
        public async Task<Result<SuccessResponseDto>> DeleteAsync(long id)
        {
            if (!await _userRepository.ExistsAsync(id))
            {
                return Result.Fail<SuccessResponseDto>("User not found.");
            }

            await _userRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();

            return Result.Ok(new SuccessResponseDto { Message = "User deleted successfully." });
        }

        /// <inheritdoc/>
        public async Task<Result<PaginatedList<UserDto>>> GetAsync(QueryFilterDto queryFilter)
        {
            var paginatedList = await _userRepository.GetAsync(queryFilter);

            if (paginatedList == null || paginatedList.Items == null)
            {
                return Result.Fail<PaginatedList<UserDto>>("Error retrieving users.");
            }

            var userDtos = _mapper.Map<List<UserDto>>(paginatedList.Items);

            return Result.Ok(new PaginatedList<UserDto>(userDtos, paginatedList.TotalCount));
        }

        /// <inheritdoc/>
        public async Task<Result<List<UserDto>>> GetTechniciansAsync()
        {
            var tecnitiansDtos = _mapper.Map<List<UserDto>>(await _userRepository.GetAllTechniciansAsync());

            return Result.Ok(new List<UserDto>(tecnitiansDtos));
        }

        /// <inheritdoc/>
        public async Task<Result<UserDto>> GetByIdAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return Result.Fail<UserDto>("User not found.");
            }

            var response = _mapper.Map<UserDto>(user);
            return Result.Ok(response);
        }

        /// <inheritdoc/>
        public async Task<Result<long>> VerifyUserAsync(string userId)
        {
            if (!long.TryParse(userId, out long parsedUserId))
            {
                return Result.Fail<long>("Invalid user ID.");
            }

            User? user = await _userRepository.GetByIdAsync(parsedUserId);

            if (user == null)
            {
                return Result.Fail<long>("User not found.");
            }
            
            return Result.Ok(parsedUserId);
        }

        /// <inheritdoc/>
        public async Task<Result<SuccessResponseDto>> UpdateUserTypeAsync(long userId, UserType userType)
        {
            User? user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return Result.Fail<SuccessResponseDto>("User not found.");
            }

            user.UserType = userType;
            _userRepository.Update(user);
            await _unitOfWork.SaveAsync();

            return Result.Ok(new SuccessResponseDto { Message = "User type updated successfully." });
        }
    }
}
