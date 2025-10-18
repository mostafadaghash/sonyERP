using Microsoft.EntityFrameworkCore;

namespace SonyERP.Data;

public interface IRepository<T> where T : class
{
    IQueryable<T> Query();
    Task<T?> GetAsync(params object[] keys);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _db;
    public Repository(ApplicationDbContext db) => _db = db;

    public IQueryable<T> Query() => _db.Set<T>().AsQueryable();
    public Task<T?> GetAsync(params object[] keys) => _db.Set<T>().FindAsync(keys).AsTask();
    public Task AddAsync(T entity) => _db.Set<T>().AddAsync(entity).AsTask();
    public void Update(T entity) => _db.Set<T>().Update(entity);
    public void Remove(T entity) => _db.Set<T>().Remove(entity);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;
    public UnitOfWork(ApplicationDbContext db) => _db = db;
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
