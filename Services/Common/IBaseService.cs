using Microsoft.AspNetCore.JsonPatch;

namespace TruckDispatcherApi.Services.Common
{
    public interface IBaseService<D> where D : class
    {
        /// <summary>
        /// Creates object of type T in a database using repository
        /// </summary>
        /// <param name="modelDto"></param>
        /// <returns>Object of type T</returns>
        Task<D> CreateAsync(D modelDto);

        /// <summary>
        /// Deletes object of type T from a database using repository and string id
        /// </summary>
        /// <param name="id">string identifier</param>
        /// <returns>void</returns>
        Task DeleteAsync(string id);

        /// <summary>
        /// Gets object of type T from repository using int identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Instance of type T</returns>
        Task<D?> GetAsync(string id);

        /// <summary>
        /// Updates partly object of type T (or array) in a database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        Task<D?> PartialUpdateAsync(string id, JsonPatchDocument<object> patchDocument);

        /// <summary>
        /// Updates object of type D in a database using repository
        /// </summary>
        /// <param name="modelDto"></param>
        /// <returns></returns>
        Task<D> UpdateAsync(D modelDto);
    }
}
