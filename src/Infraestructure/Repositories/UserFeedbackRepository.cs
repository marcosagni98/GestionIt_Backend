using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class UserFeedbackRepository : IUserFeedbackRepository
{
    private readonly DbContext _context;
    private DbSet<UserFeedback> _dbSet;

    public UserFeedbackRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<UserFeedback>();
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