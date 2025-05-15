using TruckDispatcherApi.Services.Common;

namespace TruckDispatcherApi.Services
{
    public interface IInvoiceService : IBaseService<InvoiceDto>
    {
        Task<ISearchParams<InvoiceDto>> GetAsync(ISearchParams<InvoiceDto> searchParams);

        Task<int> GenerateInvoiceNumber();
    }
}
