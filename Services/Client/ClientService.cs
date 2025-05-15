using AutoMapper;
using System.Linq.Expressions;
using TruckDispatcherApi.Data;
using TruckDispatcherApi.Library;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class ClientService(IMapper mapper,
        IRepository<Client> repository,
        IServiceResult<Client> serviceResult) : AppBaseService<Client, ClientDto>(mapper, repository, serviceResult), IClientService
    {
        public async Task<ClientSearchParams<ClientDto>> GetAsync(ClientSearchParams<ClientDto> clientSearchParams)
        {
            // filtering by part of Name, DOT, City, ClientStatus or AppRoles
            var filters = new List<Expression<Func<Client, bool>>>();
            if (!string.IsNullOrEmpty(clientSearchParams.SearchCriteria))
                filters.Add(c => c.Name.Contains(clientSearchParams.SearchCriteria)
                || c.DotNumber.Contains(clientSearchParams.SearchCriteria)
                || c.City.Contains(clientSearchParams.SearchCriteria));
            filters.Add(c => c.ClientStatus == clientSearchParams.ClientStatus);
            filters.Add(c => c.AppRoles == clientSearchParams.AppRoles);

            // no included entities

            // sorting Name, City, ClientStatus, AppRoles, DOT, CreatedAt or InvitedAt
            Func<IQueryable<Client>, IOrderedQueryable<Client>>? orderBy = null;
            if (clientSearchParams.Order != OrderType.None)
            {
                orderBy = clientSearchParams.SortField switch
                {
                    "Name" => clientSearchParams.Order == OrderType.Ascending ? q => q.OrderBy(c => c.Name) : q => q.OrderByDescending(c => c.Name),
                    "City" => clientSearchParams.Order == OrderType.Ascending ? q => q.OrderBy(c => c.City) : q => q.OrderByDescending(c => c.City),
                    "ClientStatus" => clientSearchParams.Order == OrderType.Ascending ? q => q.OrderBy(c => c.ClientStatus) : q => q.OrderByDescending(c => c.ClientStatus),
                    "AppRoles" => clientSearchParams.Order == OrderType.Ascending ? q => q.OrderBy(c => c.AppRoles) : q => q.OrderByDescending(c => c.AppRoles),
                    "DOT" => clientSearchParams.Order == OrderType.Ascending ? q => q.OrderBy(c => c.DotNumber) : q => q.OrderByDescending(c => c.DotNumber),
                    "CreatedAt" => clientSearchParams.Order == OrderType.Ascending ? q => q.OrderBy(c => c.CreatedAt) : q => q.OrderByDescending(c => c.CreatedAt),
                    _ => clientSearchParams.Order == OrderType.Ascending ? q => q.OrderBy(c => c.InvitedAt) : q => q.OrderByDescending(c => c.InvitedAt),
                };
            }

            await Search(clientSearchParams, filters: filters, orderBy: orderBy);

            return clientSearchParams;
        }
    }
}
