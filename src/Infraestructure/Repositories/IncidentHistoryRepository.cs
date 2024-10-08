using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class IncidentHistoryRepository : BaseRepository<IncidentHistory>, IIncidentHistoryRepository
{
    private readonly AppDbContext _context; 
    private readonly DbSet<IncidentHistory> _dbSet;
    private readonly IMapper _mapper;

    public IncidentHistoryRepository(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
        _context = context;
        _dbSet = _context.Set<IncidentHistory>();
        _mapper = mapper;
    }

    #region Dispose
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _context?.Dispose();
        }
    }
    #endregion

}
