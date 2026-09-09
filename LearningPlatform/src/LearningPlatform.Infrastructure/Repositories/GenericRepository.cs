using System.Linq.Expressions;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Repositories;
using LearningPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    public GenericRepository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id) => await DbSet.FindAsync(id);

    public async Task<IReadOnlyList<T>> GetAllAsync() => await DbSet.ToListAsync();

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
        await DbSet.Where(predicate).ToListAsync();

    public async Task AddAsync(T entity) => await DbSet.AddAsync(entity);

    public void Update(T entity) => DbSet.Update(entity);

    public void Remove(T entity) => DbSet.Remove(entity);

    public IQueryable<T> Query() => DbSet.AsQueryable();
}
