using Domain.Interfaces.Repositories;

namespace Infrastructure;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }
}