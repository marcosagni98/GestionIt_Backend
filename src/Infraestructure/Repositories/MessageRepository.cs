using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class MessageRepository(AppDbContext context, IMapper mapper) : GenericRepository<Message>(context, mapper), IMessageRepository
{

}