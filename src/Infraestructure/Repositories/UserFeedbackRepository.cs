using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class UserFeedbackRepository(AppDbContext context, IMapper mapper) : GenericRepository<UserFeedback>(context, mapper), IUserFeedbackRepository
{

}