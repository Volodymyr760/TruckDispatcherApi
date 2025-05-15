using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;
using TruckDispatcherApi.Data;

namespace TruckDispatcherApi.Services
{
    public abstract class AppBaseService<TModel, Dto>(
        IMapper mapper,
        IRepository<TModel> repository,
        IServiceResult<TModel> serviceResult) where TModel : class where Dto : class
    {
        public IMapper Mapper { get; } = mapper;
        public IRepository<TModel> Repository { get; set; } = repository;
        public IServiceResult<TModel> ServiceResult { get; set; } = serviceResult;

        public async Task Search(
            ISearchParams<Dto> searchParams,
            List<Expression<Func<TModel, bool>>>? filters = null,
            List<Expression<Func<TModel, object>>>? navProperties = null,
            Func<IQueryable<TModel>, IOrderedQueryable<TModel>>? orderBy = null)
        {
            ServiceResult = await Repository.GetAsync(searchParams.PageSize, searchParams.CurrentPage, filters, navProperties, orderBy);

            searchParams.ItemList = Mapper.Map<IEnumerable<Dto>>(ServiceResult.Items);
            searchParams.PageCount = ServiceResult == null ? 0 : searchParams.PageSize == 0 ? 1 : Convert.ToInt32(Math.Ceiling((double)ServiceResult.TotalCount / searchParams.PageSize));
            searchParams.TotalItemsCount = ServiceResult == null ? 0 : ServiceResult.TotalCount;
        }

        /// <summary>
        /// Gets object from repository using int identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Data transfer object of type D</returns>
        public async Task<Dto?> GetAsync(string id) => Mapper.Map<Dto>(await Repository.GetAsync(id));

        /// <summary>
        /// Creates object of type D in a database using repository
        /// </summary>
        /// <param name="modelDto">Data transfer object of type D</param>
        /// <returns>Created Dto object</returns>
        public async Task<Dto> CreateAsync(Dto modelDto)
        {
            var model = Mapper.Map<TModel>(modelDto);

            return Mapper.Map<Dto>(await Repository.CreateAsync(model));
        }

        /// <summary>
        /// Updates object of type D in a database using repository
        /// </summary>
        /// <param name="modelDto"></param>
        /// <returns>Updated Dto object</returns>
        public async Task<Dto> UpdateAsync(Dto modelDto)
        {
            var model = Mapper.Map<TModel>(modelDto);

            return Mapper.Map<Dto>(await Repository.UpdateAsync(model));
        }

        /// <summary>
        /// Deletes object of type D from a database using repository
        /// </summary>
        /// <param name="id"></param>
        /// <returns>void</returns>
        public async Task DeleteAsync(string id) => await Repository.DeleteAsync(id);

        /// <summary>
        /// Updates partly object of type T (or array) in a database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patchDocument"></param>
        /// <returns>Updated Updated Dto object</returns>
        public async Task<Dto?> PartialUpdateAsync(string id, JsonPatchDocument<object> patchDocument)
        {
            TModel? model = await Repository.GetAsync(id);
            if (model != null)
            {
                patchDocument.ApplyTo(model);
                return Mapper.Map<Dto>(await Repository.SaveAsync(model));
            }

            return null;
        }
    }
}
