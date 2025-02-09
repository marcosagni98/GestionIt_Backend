using Domain.Interfaces.Repositories;

namespace Infrastructure;

public class EfUnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }
}