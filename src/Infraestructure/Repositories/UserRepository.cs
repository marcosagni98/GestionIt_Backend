using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class UserRepository(AppDbContext context, IMapper mapper) : GenericRepository<User>(context, mapper), IUserRepository
{

}

