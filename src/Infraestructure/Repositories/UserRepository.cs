using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<User> _dbSet;
    private readonly IMapper _mapper;

    public UserRepository(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
        _dbContext = context;
        _mapper = mapper;
        _dbSet = _dbContext.Set<User>();
    }

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

