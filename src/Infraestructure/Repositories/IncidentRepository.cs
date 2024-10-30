
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace Infraestructure.Repositories;

public class IncidentRepository(AppDbContext context, IMapper mapper) : GenericRepository<Incident>(context, mapper), IIncidentRepository
{

}

