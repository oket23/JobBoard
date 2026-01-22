using JobBoard.Identity.Domain.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Identity.Infrastructure.Repositories.Base;

public abstract class RepositoryBase<TEntity> where TEntity : class
{
    private readonly JobBoardIdentityContext _context;
    protected IQueryable<T> Set<T>() where T : class => _context.Set<T>().AsNoTracking();

    protected RepositoryBase(JobBoardIdentityContext context)
    {
        _context = context;
    }

    public virtual ValueTask<TEntity?> GetById(int id, CancellationToken cancellationToken)
    {
        return _context.Set<TEntity>().FindAsync([id], cancellationToken);
    }

    public virtual void Add(TEntity entity)
    {
        _context.Add(entity);
    }

    public virtual void Update(TEntity entity)
    {
        _context.Update(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        if(entity is BaseEntity baseEntity)
            baseEntity.DeletedAt = DateTime.Now;
        else
            _context.Remove(entity);
    }
}