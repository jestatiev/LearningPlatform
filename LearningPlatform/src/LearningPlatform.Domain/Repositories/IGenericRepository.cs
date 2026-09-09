using System.Linq.Expressions;
using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Domain.Repositories;

// Repository Pattern - generičko sučelje za osnovne CRUD operacije nad bilo kojim entitetom
public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    IQueryable<T> Query(); // omogućuje Include() pozive iz servisnog sloja kad zatreba
}
