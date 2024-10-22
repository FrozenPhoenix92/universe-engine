using Microsoft.EntityFrameworkCore;

namespace Universe.Core.Data;

/// <summary>
/// Базовый класс контекста данных.
/// </summary>
public abstract class DbContextCore : DbContext, IDbContext
{
    public DbContextCore(DbContextOptions options) : base(options) 
    {
    }
}
