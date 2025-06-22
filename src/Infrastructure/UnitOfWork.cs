using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private IDbContextTransaction? _currentTransaction;

    public bool IsTransactionActive => _currentTransaction != null;

    public async Task SaveAsync() => await context.SaveChangesAsync();

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }
        _currentTransaction = await context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }
        await _currentTransaction.CommitAsync();
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public async Task RollbackTransactionAsync()
    {
        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }
        await _currentTransaction.RollbackAsync();
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }
}