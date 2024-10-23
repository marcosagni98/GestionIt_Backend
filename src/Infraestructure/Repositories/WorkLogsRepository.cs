using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class WorkLogsRepository : GenericRepository<WorkLog>, IWorkLogRepository
{
    private readonly AppDbContext _context;
    private DbSet<WorkLog> _dbSet;
    private readonly IMapper _mapper;

    public WorkLogsRepository(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<WorkLog>();
        _mapper = mapper;
    }
}

