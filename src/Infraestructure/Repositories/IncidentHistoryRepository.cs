using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class IncidentHistoryRepository : IIncidentHistoryRepository
{
    private readonly AppDbContext _context; 
    private DbSet<IncidentHistory> _dbSet;

    public IncidentHistoryRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<IncidentHistory>(); 
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
