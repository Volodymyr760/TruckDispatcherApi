using AutoMapper;
using System.Linq.Expressions;
using TruckDispatcherApi.Data;
using TruckDispatcherApi.Library;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class HeatmapService(IMapper mapper,
        IRepository<Heatmap> repository,
        IServiceResult<Heatmap> serviceResult) : AppBaseService<Heatmap, HeatmapDto>(mapper, repository, serviceResult), IHeatmapService
    {
        public async Task<HeatmapSearchParams<HeatmapDto>> GetAsync(HeatmapSearchParams<HeatmapDto> searchParams)
        {
            // filtering
            var filters = new List<Expression<Func<Heatmap, bool>>>
            {
                hm => hm.DayType == searchParams.DayType,
                hm => hm.Equipment == searchParams.Equipment
            };

            // Include HeatmapStates
            var navProperties = new List<Expression<Func<Heatmap, object>>> { hm => hm.HeatmapStates };

            ServiceResult = await Repository.GetAsync(int.MaxValue, 1, filters, navProperties, null);
            var heatmapDto = Mapper.Map<HeatmapDto>(ServiceResult.Items.ToList()[0]);

            // sorting HeatmapStates in HeatmapDto from db by State, Ranq, RPM pickup, RPM delivery, pickups amoumt or delivery amount
            if (searchParams.Order != OrderType.None)
            {
                switch (searchParams.SortField)
                {
                    case "Ranq":
                        heatmapDto.HeatmapStates = searchParams.Order == OrderType.Ascending ?
                            heatmapDto.HeatmapStates.OrderBy(hms => hms.Ranq).ToList() :
                            heatmapDto.HeatmapStates.OrderByDescending(hms => hms.Ranq).ToList();
                        break;
                    case "RPM pickup":
                        heatmapDto.HeatmapStates = searchParams.Order == OrderType.Ascending ?
                            heatmapDto.HeatmapStates.OrderBy(hms => hms.AveragePickupRate).ToList() :
                            heatmapDto.HeatmapStates.OrderByDescending(hms => hms.AveragePickupRate).ToList();
                        break;
                    case "RPM delivery":
                        heatmapDto.HeatmapStates = searchParams.Order == OrderType.Ascending ?
                            heatmapDto.HeatmapStates.OrderBy(hms => hms.AverageDeliveryRate).ToList() :
                            heatmapDto.HeatmapStates.OrderByDescending(hms => hms.AverageDeliveryRate).ToList();
                        break;
                    case "Pickups":
                        heatmapDto.HeatmapStates = searchParams.Order == OrderType.Ascending ?
                            heatmapDto.HeatmapStates.OrderBy(hms => hms.PickupsAmount).ToList() :
                            heatmapDto.HeatmapStates.OrderByDescending(hms => hms.PickupsAmount).ToList();
                        break;
                    case "Deliveries":
                        heatmapDto.HeatmapStates = searchParams.Order == OrderType.Ascending ?
                            heatmapDto.HeatmapStates.OrderBy(hms => hms.DeliveriesAmount).ToList() :
                            heatmapDto.HeatmapStates.OrderByDescending(hms => hms.DeliveriesAmount).ToList();
                        break;
                    default: // by State
                        heatmapDto.HeatmapStates = searchParams.Order == OrderType.Ascending ?
                            heatmapDto.HeatmapStates.OrderBy(hms => hms.State).ToList() :
                            heatmapDto.HeatmapStates.OrderByDescending(hms => hms.State).ToList();
                        break;
                }
            }

            searchParams.TotalItemsCount = ServiceResult.TotalCount;
            searchParams.ItemList = [heatmapDto];

            return searchParams;
        }

        public async Task ChangeUpdatedATAsync(HeatmapDto heatmap)
        {
            Expression<Func<Heatmap, bool>> searchQuery = a => a.Id == heatmap.Id;
            var heatmapFromDB = await Repository.GetAsync(searchQuery, []);
            if (heatmapFromDB != null)
            {
                heatmapFromDB.UpdatedAt = DateTime.UtcNow;
                await Repository.SaveAsync(heatmapFromDB);
            }
        }
    }
}
