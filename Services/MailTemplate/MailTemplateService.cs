using AutoMapper;
using System.Linq.Expressions;
using TruckDispatcherApi.Data;
using TruckDispatcherApi.Library;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class MailTemplateService : AppBaseService<MailTemplate, MailTemplateDto>, IMailTemplateService
    {
        public MailTemplateService(IMapper mapper,
            IRepository<MailTemplate> repository,
            IServiceResult<MailTemplate> serviceResult) : base(mapper, repository, serviceResult) { }

        public async Task<ISearchParams<MailTemplateDto>> GetAsync(ISearchParams<MailTemplateDto> searchParams)
        {
            // sorting by Subject or MessagePlainText
            Func<IQueryable<MailTemplate>, IOrderedQueryable<MailTemplate>>? orderBy = null;
            if (searchParams.Order != OrderType.None)
            {
                orderBy = searchParams.SortField switch
                {
                    "MessagePlainText" => searchParams.Order == OrderType.Ascending ? o => o.OrderBy(mt => mt.MessagePlainText) : o => o.OrderByDescending(mt => mt.MessagePlainText),
                    _ => searchParams.Order == OrderType.Ascending ? o => o.OrderBy(mt => mt.Subject) : o => o.OrderByDescending(mt => mt.Subject),
                };
            }

            await Search(searchParams, filters: null, navProperties: null, orderBy: orderBy);

            return searchParams;
        }

        public async Task<MailTemplateDto> GetByMailTemplateKey(MailTemplateKey mailTemplateKey)
        {
            Expression<Func<MailTemplate, bool>> searchQuery = mt => mt.MailTemplateKey == mailTemplateKey;
            var mailTemplate = await Repository.GetAsync(searchQuery, navigationProperties: []);

            return Mapper.Map<MailTemplateDto>(await Repository.GetAsync(searchQuery, []));
        }
    }
}
