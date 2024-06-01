using Microsoft.EntityFrameworkCore;
using User_API.Models;


namespace User_API.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}