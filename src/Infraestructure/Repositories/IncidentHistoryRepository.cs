using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class IncidentHistoryRepository(AppDbContext context, IMapper mapper) : GenericRepository<IncidentHistory>(context, mapper), IIncidentHistoryRepository
{

}
