using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.Users.Request;
using Application.Dtos.CRUD.Users.Response;
using Application.Interfaces;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers.v1
{
    /// <summary>
    /// Controller for managing user-related operations.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </remarks>
    /// <param name="userService">The user service.</param>
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class UserController(IUserService userService) : BaseApiController
    {
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <param name="addRequestDto">The data for the new user.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResponseDto))]
        public async Task<IActionResult> AddAsync([FromBody] UserAddRequestDto addRequestDto)
        {
            var result = await _userService.AddAsync(addRequestDto);
            if (result.IsFailed)
            {
                return BadRequest(result.Errors);
            }

            return CreatedAtAction(nameof(GetByIdAsync), null, result.Value);
        }

        /// <summary>
        /// Gets a list of users.
        /// </summary>
        /// <returns>A list of users.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<UserResponseDto>))]
        public async Task<IActionResult> GetAsync([FromQuery] QueryFilterDto queryFilter)
        {
            var result = await _userService.GetAsync(queryFilter);
            if (result.IsFailed)
            {
                return NotFound(result.Errors);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Gets a user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The requested user.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _userService.GetByIdAsync(id);
            if (result.IsFailed)
            {
                return NotFound(result.Errors);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="updateRequestDto">The updated data for the user.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UserUpdateRequestDto updateRequestDto)
        {
            var result = await _userService.UpdateAsync(id, updateRequestDto);
            if (result.IsFailed)
            {
                return NotFound(result.Errors);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _userService.DeleteAsync(id);
            if (result.IsFailed)
            {
                return NotFound(result.Errors);
            }

            return Ok(result.Value);
        }
    }
}
