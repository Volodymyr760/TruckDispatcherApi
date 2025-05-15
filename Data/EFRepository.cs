using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TruckDispatcherApi.Services;

namespace TruckDispatcherApi.Data
{
    public class EFRepository<TModel>(TruckDispDbContext context) : IRepository<TModel> where TModel : class
    {
        private readonly TruckDispDbContext context = context;

        private DbSet<TModel> Set { get; set; } = context.Set<TModel>();

        public async Task<TModel> CreateAsync(TModel model)
        {
            try
            {
                Set.Add(model);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new CreateEntityFailedException(typeof(TModel), ex);
            }

            return model;
        }

        public async Task DeleteAsync(string id)
        {
            try
            {
                TModel? model = await Set.FindAsync(id);
                if (model != null)
                {
                    Set.Remove(model);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new DeleteEntityFailedException(typeof(TModel), ex);
            }
        }

        public async Task<TModel?> GetAsync(string id)
        {
            try
            {
                return await Set.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new RetrieveEntityFailedException<TModel>(id.ToString(), ex);
            }
        }

        public async Task<TModel?> GetAsync(Expression<Func<TModel, bool>> query, List<Expression<Func<TModel, object>>> navProperties)
        {
            try
            {
                IQueryable<TModel> dbSet = Set;
                if (query != null) dbSet = dbSet.Where(query);
                if (navProperties.Count > 0)
                {
                    foreach (Expression<Func<TModel, object>> property in navProperties)
                        dbSet = dbSet.Include(property);
                }
                return await dbSet.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new RetrieveEntitiesQueryFailedException(typeof(TModel), ex);
            }
        }

        public async Task<List<TModel>> GetAsync(string sqlQuery, SqlParameter[]? parameters)
        {
            try
            {
                IQueryable<TModel> result = parameters == null ? Set.FromSqlRaw(sqlQuery) :
                    Set.FromSqlRaw(sqlQuery, parameters);
                return await result.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RetrieveEntitiesQueryFailedException(typeof(TModel), ex);
            }
        }

        public async Task<IServiceResult<TModel>> GetAsync(
            int limit,
            int page = 1,
            List<Expression<Func<TModel, bool>>>? filters = null,            
            List<Expression<Func<TModel, object>>>? navProperties = null,
            Func<IQueryable<TModel>, IOrderedQueryable<TModel>>? orderBy = null)
        {
            var serviceResult = new ServiceResult<TModel>() { };
            IQueryable<TModel> dbSet = Set;
            try
            {
                if (filters?.Count > 0) foreach (var filter in filters) dbSet = dbSet.Where(filter);
                if (navProperties != null) foreach (var property in navProperties)
                        dbSet = dbSet.Include(property);
                if (orderBy != null) dbSet = orderBy(dbSet);
                serviceResult.TotalCount = dbSet.Count();
                dbSet = dbSet.Skip((page - 1) * limit).Take(limit);
                serviceResult.Items = await dbSet.ToListAsync();

                return serviceResult;
            }
            catch (Exception ex)
            {
                throw new RetrieveEntitiesQueryFailedException(typeof(TModel), ex);
            }
        }

        public async Task<TModel> UpdateAsync(TModel model)
        {
            try
            {
                Set.Attach(model);
                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new UpdateEntityFailedException(typeof(TModel), ex);
            }

            return model;
        }

        public async Task<TModel> SaveAsync(TModel model)
        {
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new UpdateEntityFailedException(typeof(TModel), ex);
            }

            return model;
        }

        public async Task<bool> GetBoolValue(string sqlQuery, SqlParameter[] parameters)
        {
            await context.Database.ExecuteSqlRawAsync(sqlQuery, parameters);
            bool result = false;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] != null && parameters[i].ParameterName == "@boolResult")
                {
                    result = (bool)parameters[i].Value;
                    break;
                }
            }

            return result;
        }

        public async Task<int> GetIntValue(string sqlQuery, SqlParameter[] parameters)
        {
            await context.Database.ExecuteSqlRawAsync(sqlQuery, parameters);
            int result = 0;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] != null && parameters[i].ParameterName == "@intResult")
                {
                    result = (int)parameters[i].Value;
                    break;
                }
            }
            return result;
        }

        public async Task<long> GetLongValue(string sqlQuery, SqlParameter[] parameters)
        {
            await context.Database.ExecuteSqlRawAsync(sqlQuery, parameters);
            long result = 0L;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] != null && parameters[i].ParameterName == "@boolResult")
                {
                    result = (long)parameters[i].Value;
                    break;
                }
            }
            return result;
        }

        public async Task<string> GetStringValue(string sqlQuery, SqlParameter[] parameters)
        {
            await context.Database.ExecuteSqlRawAsync(sqlQuery, parameters);
            string result = string.Empty;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] != null && parameters[i].ParameterName == "@boolResult")
                {
                    result = (string)parameters[i].Value;
                    break;
                }
            }
            return result;
        }

        public async Task ExecuteSQLQueryAsync(string sqlQuery, SqlParameter[] parameters) =>
            await context.Database.ExecuteSqlRawAsync(sqlQuery, parameters);
    }
}
