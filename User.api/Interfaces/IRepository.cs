using System.Linq.Expressions;

namespace User_API.Interfaces;

public interface IRepository<T>
{
    public ICollection<T> Find(Expression<Func<T, bool>> predicate);
    public ICollection<T> GetAll();
    public T? GetById(Guid id);
    public void Create(T entity);
    public void Update(T entity);
    public void Remove(T entity);
}