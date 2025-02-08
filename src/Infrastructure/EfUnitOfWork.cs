using Domain.Interfaces.Repositories;

namespace Infrastructure;

public class EfUnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task CommitAsync()
    {
        await context.SaveChangesAsync();
    }
}