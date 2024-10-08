using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class MessageRepository : BaseRepository<Message>, IMessageRepository
{
    private readonly AppDbContext _context;
    private DbSet<Message> _dbSet;
    private readonly IMapper _mapper;

    public MessageRepository(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<Message>();
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