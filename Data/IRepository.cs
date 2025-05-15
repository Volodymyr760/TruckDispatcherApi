using Microsoft.Data.SqlClient;
using System.Linq.Expressions;
using TruckDispatcherApi.Services;

namespace TruckDispatcherApi.Data
{
    public interface IRepository<TModel> where TModel : class
    {
        /// <summary>
        /// Creates a new TModel item.
        /// </summary>
        /// <param name="model">TModel instance</param>
        /// <returns>Created TModel object with identifier</returns>
        Task<TModel> CreateAsync(TModel model);

        /// <summary>
        /// Deletes TModel item by string identifier.
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Completed Task</returns>
        Task DeleteAsync(string id);

        /// <summary>
        /// Gets a specific TModel Item. In use by AppBaseService.
        /// </summary>
        /// <param name="id">String identifier</param>
        /// <returns>TModel instance</returns>
        Task<TModel?> GetAsync(string id);

        /// <summary>
        /// Gets a specific TModel Item with Expressions for complex object.
        /// </summary>
        /// <param name="query">Delegate for Where-condition like 'e => e.Id == id'</param>
        /// <param name="navigationProperties">Array of delegates to include navigation properties</param>
        /// <returns>TModel instance</returns>
        Task<TModel?> GetAsync(Expression<Func<TModel, bool>> query, List<Expression<Func<TModel, object>>> navigationProperties);

        /// <summary>
        /// Gets a list of objects from stored procedures
        /// </summary>
        /// <param name="sqlQuery">Calls stored procedures</param>
        /// <param name="parameters">Array of params passed to the stored procedure, null or array with 1 or more parameters</param>
        /// <returns>List of objects</returns>
        Task<List<TModel>> GetAsync(string sqlQuery, SqlParameter[]? parameters);

        Task<IServiceResult<TModel>> GetAsync(
            int limit,
            int page = 1,
            List<Expression<Func<TModel, bool>>>? filters = null,
            List<Expression<Func<TModel, object>>>? navProperties = null,
            Func<IQueryable<TModel>, IOrderedQueryable<TModel>>? orderBy = null
            );

        /// <summary>
        /// Gets the bool result, using shot store procedure instead EntityFramework
        /// </summary>
        /// <param name="sqlQuery">SQL query to stored procedure. Includes @id (input) and @boolResult (output) params.</param>
        /// <param name="parameters">Formalized array of SqlParameters</param>
        /// <remarks>
        /// 
        /// Sample stored procedure:
        ///     CREATE OR ALTER PROCEDURE [dbo].[sp_checkAppFileById]
        ///     @id nvarchar(450), @boolResult bit output,
        ///     AS
        ///        DECLARE @amount int
        ///        SELECT @amount = COUNT(Id)
        ///        FROM AppFiles
        ///        WHERE Id = @id
        ///
        ///        SET @boolResult = (SELECT CASE WHEN @amount > 0 THEN 1 ELSE 0 END)
        ///
        ///        RETURN @boolResult
        ///     
        /// Sample SQL query: "EXEC sp_checkAppFileById @id, @boolResult OUTPUT"
        /// 
        /// Sample array of parameters:
        ///     SqlParameter[] parameters =
        ///         {
        ///             new SqlParameter("@id", SqlDbType.NVarChar) { Value = id },
        ///             new SqlParameter("@boolResult", SqlDbType.Bit) {Direction = ParameterDirection.Output}
        ///         };
        /// 
        /// </remarks>
        /// <returns>True - if exists: output contains more than 0 rows, False - if not</returns>
        Task<bool> GetBoolValue(string sqlQuery, SqlParameter[] parameters);

        Task<int> GetIntValue(string sqlQuery, SqlParameter[] parameters);

        Task<long> GetLongValue(string sqlQuery, SqlParameter[] parameters);

        Task<string> GetStringValue(string sqlQuery, SqlParameter[] parameters);

        /// <summary>
        /// Saves the entity, tracked by EntityFramework
        /// </summary>
        /// <param name="model">The object, which previously got from the db and is on tracking by Entity Framework</param>
        /// <returns>Saved object</returns>
        Task<TModel> SaveAsync(TModel model);

        Task<TModel> UpdateAsync(TModel model);

        Task ExecuteSQLQueryAsync(string sqlQuery, SqlParameter[] parameters);
    }
}
