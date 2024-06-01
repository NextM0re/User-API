using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using User_API.Data;
using User_API.Interfaces;
using User_API.Models;

namespace User_API.Repositories;
 
public class RamRepository<T> : IRepository<T> where T : class
{
    public RamRepository (DataContext context)
    {
        _context = context;
        CreateAdmin();
    }
    
    private readonly DataContext _context;
    
    public ICollection<T> Find(Expression<Func<T, bool>> predicate)
    {
        return _context.Set<T>().Where(predicate).ToArray();
    }
     
    public ICollection<T> GetAll()
    {
        return _context.Set<T>().ToArray();
    }
 
    public T? GetById(Guid id)
    {
        return _context.Set<T>().Find(id);
    }
 
    public void Create(T entity)
    {
        _context.Set<T>().Add(entity);
        _context.SaveChanges();
    }
 
    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
        _context.SaveChanges();
    }
 
    public void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
        _context.SaveChanges();
    }
    
    private void CreateAdmin()
    {
        var adminExists = _context.Users.Any(n => n.UserName == "adminadmin");
        
        if (adminExists)
            return;
        
        var admin = new User(
            "adminadmin",
            BCrypt.Net.BCrypt.HashPassword("admin_11"),
            "Steve_Jobs",
            1,
            new DateTime(1955, 2, 24),
            true,
            DateTime.Now, 
            "Default"
        );
        
        _context.Set<User>().Add(admin);
        _context.SaveChanges();
    }
}