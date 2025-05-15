using TruckDispatcherApi.Library;
using TruckDispatcherApi.Services.Common;

namespace TruckDispatcherApi.Services
{
    public interface IMailTemplateService : IBaseService<MailTemplateDto>
    {
        Task<ISearchParams<MailTemplateDto>> GetAsync(ISearchParams<MailTemplateDto> searchParams);

        Task<MailTemplateDto> GetByMailTemplateKey(MailTemplateKey mailTemplateKey);
    }
}
