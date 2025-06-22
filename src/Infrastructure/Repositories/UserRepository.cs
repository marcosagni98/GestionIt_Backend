using AutoMapper;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class UserRepository(AppDbContext context) : GenericRepository<User>(context), IUserRepository
{
    private readonly DbSet<User> _dbSet = context.Set<User>();

    /// <inheritdoc/>
    public override async Task<PaginatedList<User>> GetAsync(QueryFilterDto queryFilter)
    {
        List<string> searchParameters = [
            "Name",
            "Email",
        ];

        GetCorrectQueryFilterOrderBy(queryFilter);

        var query = _dbSet.AsQueryable();

        var totalCount = await query
            .WhereActive()
            .CountAsync(queryFilter, searchParameters);

        var items = await query
            .ApplyQueryFilterAndActive(queryFilter, searchParameters)
            .ToListAsync();

        return new PaginatedList<User>(items, totalCount);
    }



    #region GetAsync private functions
    /// <summary>
    /// Transforms sorting property names from Data Transfer Object (DTO) conventions to corresponding entity navigation property names.
    /// This method maps client-side sorting properties to the correct database entity property paths for efficient querying.
    /// </summary>
    /// <param name="queryFilter">The query filter containing sorting parameters to be transformed.</param>
    /// /// <remarks>
    /// Supported mappings include:
    /// - "UserName" -> "User.Name"
    /// - "UserId" -> "User.Id"
    /// - "TechnicianName" -> "Technician.Name"
    /// - "TechnicianId" -> "Technician.Id"
    /// 
    /// If no matching mapping is found, the original OrderBy value is preserved.
    /// </remarks>
    private static void GetCorrectQueryFilterOrderBy(QueryFilterDto queryFilter)
    {
        if (!string.IsNullOrWhiteSpace(queryFilter?.OrderBy))
        {
            queryFilter.OrderBy = queryFilter.OrderBy switch
            {
                "userName" => "User.Name",
                "userId" => "User.Id",
                "technicianName" => "Technician.Name",
                "technicianId" => "Technician.Id",
                _ => queryFilter.OrderBy
            };
        }
    }

    #endregion

    /// <inheritdoc/>
    public Task<bool> EmailExistsAsync(string email)
    {
        return _dbSet.Where(x => x.Email == email && x.Active == true).AnyAsync();
    }

    /// <inheritdoc/>
    public async Task<List<User>> GetAllTechniciansAsync()
    {
        return await _dbSet.Where(u => u.UserType == UserType.Technician || u.UserType == UserType.Admin).ToListAsync();
    }

    /// <inheritdoc/>
    public Task<User?> GetUserByEmailAsync(string email)
    {
        return _dbSet.FirstOrDefaultAsync(x => x.Email == email);
    }

    /// <inheritdoc/>
    public Task<bool> LoginAsync(string email, string password)
    {
        return _dbSet.Select(x => x.Email == email && x.Password == password && x.Active == true).AnyAsync();
    }
}

