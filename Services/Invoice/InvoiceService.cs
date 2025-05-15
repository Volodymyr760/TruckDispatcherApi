using AutoMapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq.Expressions;
using TruckDispatcherApi.Data;
using TruckDispatcherApi.Library;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class InvoiceService(IMapper mapper,
        IRepository<Invoice> repository,
        IServiceResult<Invoice> serviceResult) : AppBaseService<Invoice, InvoiceDto>(mapper, repository, serviceResult), IInvoiceService
    {
        public async Task<ISearchParams<InvoiceDto>> GetAsync(ISearchParams<InvoiceDto> searchParams)
        {
            // filtering
            //var filters = new List<Expression<Func<Invoice, bool>>> { q => q.UserId == searchParams.UserId };
            var filters = new List<Expression<Func<Invoice, bool>>>();
            if (!string.IsNullOrEmpty(searchParams.UserId)) filters.Add(q => q.UserId == searchParams.UserId);

            // sorting by Invoice No, IsRead, IsPaid or Created At
            Func<IQueryable<Invoice>, IOrderedQueryable<Invoice>>? orderBy = null;
            if (searchParams.Order != OrderType.None)
            {
                orderBy = searchParams.SortField switch
                {
                    "Invoice No" => searchParams.Order == OrderType.Ascending ? o => o.OrderBy(i => i.InvoiceNo) : o => o.OrderByDescending(i => i.InvoiceNo),
                    "Read" => searchParams.Order == OrderType.Ascending ? o => o.OrderBy(i => i.IsRead) : o => o.OrderByDescending(i => i.IsRead),
                    "Paid" => searchParams.Order == OrderType.Ascending ? o => o.OrderBy(i => i.IsPaid) : o => o.OrderByDescending(i => i.IsPaid),
                    _ => searchParams.Order == OrderType.Ascending ? o => o.OrderBy(i => i.CreatedAt) : o => o.OrderByDescending(i => i.CreatedAt)
                };
            }

            await Search(searchParams, filters, null, orderBy);

            return searchParams;
        }

        public async Task<int> GenerateInvoiceNumber()
        {
            SqlParameter[] parameters = { new SqlParameter("@intResult", SqlDbType.Int) { Direction = ParameterDirection.Output } };

            return await Repository.GetIntValue("EXEC sp_getMaxInvoiceNumber @intResult OUTPUT", parameters) + 1;
        }
    }
}
