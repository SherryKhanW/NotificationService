using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using Elastic.Clients.Elasticsearch;

namespace NotificationService.Repositories;

public class GenericRepository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;
    protected readonly ElasticsearchClient Elasticsearch;
    
    public GenericRepository(AppDbContext context, ElasticsearchClient elasticsearch)
    {
        Context = context;
        DbSet = context.Set<T>();
        Elasticsearch = elasticsearch;
    }
    
    protected async Task LogAsync(string message)
    {
        const string indexName = "logs-notificationservice-default";

        await Elasticsearch.IndexAsync(new
        {
            @timestamp = DateTime.UtcNow,
            serviceName = "NotificationService",
            message = message,
            repositoryName = typeof(T).Name,
            operation = "RepositoryAction",
            success = true
        }, i => i
            .Index(indexName)
            .OpType(Elastic.Clients.Elasticsearch.OpType.Create));
    }
    
    public async Task<T> CreateAsync(T entity)
    {
        DbSet.Add(entity);
        await Context.SaveChangesAsync();
        
        await LogAsync($"{typeof(T).Name} created successfully.");
        
        return entity;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        var entity = await DbSet.FindAsync(id);
        
        if (entity != null)
        {
            await LogAsync($"{typeof(T).Name} retrieved from database.");
        }
        
        return entity;
    }

    public async Task<List<T>> GetAllAsync()
    {
        var entities = await DbSet.ToListAsync();

        await LogAsync($"{typeof(T).Name} collection retrieved.");

        return entities;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        DbSet.Update(entity);

        await Context.SaveChangesAsync();

        await LogAsync($"{typeof(T).Name} updated successfully.");

        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);

        if (entity == null)

            return;

        DbSet.Remove(entity);

        await Context.SaveChangesAsync();

        await LogAsync($"{typeof(T).Name} deleted successfully.");
    }
}