using System.Linq.Expressions;
using AppTemplate.Domain.Entities;
using AppTemplate.Domain.Interfaces;
using AppTemplate.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppTemplate.Infrastructure.Data.Repositories;

public class GenericRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id)
        => await _dbSet.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.Where(predicate).ToListAsync();

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is not null)
            _dbSet.Remove(entity);
    }

    public async Task<bool> ExistsAsync(int id)
        => await _dbSet.AnyAsync(x => x.Id == id);

    public IQueryable<T> Query()
        => _dbSet.AsQueryable();
}
