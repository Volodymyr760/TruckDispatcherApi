using AutoMapper;
using System.Linq.Expressions;
using TruckDispatcherApi.Data;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class HeatmapStateService(IMapper mapper,
        IRepository<HeatmapState> repository,
        IServiceResult<HeatmapState> serviceResult) : AppBaseService<HeatmapState, HeatmapStateDto>(mapper, repository, serviceResult), IHeatmapStateService
    {
        public void GenerateHeatmapStatesAsync(HeatmapDto heatmap, IEnumerable<ImportLoadDto> importLoads)
        {
            // для кожного HeatmapState вибрати підгрупу ImportLoads і калькулювати величини
            foreach (var heatmapState in heatmap.HeatmapStates)
            {
                var pickupImportLoads = importLoads.Where(il => il.Origin.Contains(", " + heatmapState.State)).ToList();
                CalculateHeatmapStateValues(heatmapState, pickupImportLoads, true);
                var deliveryImportLoads = importLoads.Where(il => il.Destination.Contains(", " + heatmapState.State)).ToList();
                CalculateHeatmapStateValues(heatmapState, deliveryImportLoads, false);
            }

            // Кожному HeatmapState обчислити Ranq
            // Діапазон для обчислення кожного Ranq - від 0 до макс зн-ня SumPickupRates, ділимо діапазон на 5 частин.
            // Дослідним шляхом виявлено, що приблизно однакова к-сть HeatmapStates попадає в такі межі [4; 12, 32, 80, 100]
            // Ranq приймає зн-ня від 1 до 5 в залежності від частини діапазону, в який потрапляє конкретне зн-ня HeatmapState
            decimal maxOptimizedFunctionValue = heatmap.HeatmapStates.Max(hs => hs.SumPickupRates);
            heatmap.HeatmapStates = heatmap.HeatmapStates.OrderBy(hms => hms.SumPickupRates).ToList();

            foreach(var hms in heatmap.HeatmapStates)
            {
                switch (hms.SumPickupRates)
                {
                    case decimal d when (d <= maxOptimizedFunctionValue * 0.04m):
                        hms.Ranq = 1;
                        break;
                    case decimal d when (d > maxOptimizedFunctionValue * 0.04m && d <= maxOptimizedFunctionValue * 0.12m):
                        hms.Ranq = 2;
                        break;
                    case decimal d when (d > maxOptimizedFunctionValue * 0.12m && d <= maxOptimizedFunctionValue * 0.32m):
                        hms.Ranq = 3;
                        break;
                    case decimal d when (d > maxOptimizedFunctionValue * 0.32m && d <= maxOptimizedFunctionValue * 0.8m):
                        hms.Ranq = 4;
                        break;
                    default:
                        hms.Ranq = 5;
                        break;
                }
            }
        }

        public async Task ResetAsync()
        {
            ServiceResult = await Repository.GetAsync(int.MaxValue, 1, null, null, null);

            foreach (var heatmapState in ServiceResult.Items)
            {
                heatmapState.PickupsAmount = 0;
                heatmapState.SumPickupRates = 0;
                heatmapState.AveragePickupRate = 0;
                heatmapState.DeliveriesAmount = 0;
                heatmapState.SumDeliveryRates = 0;
                heatmapState.AverageDeliveryRate = 0;
                heatmapState.Ranq = 1;

                await Repository.UpdateAsync(heatmapState);
            }
        }

        public async Task SaveHeatmapStateAsync(HeatmapStateDto heatmapState)
        {
            Expression<Func<HeatmapState, bool>> searchQuery = a => a.Id == heatmapState.Id;
            var heatmapStateFromDB = await Repository.GetAsync(searchQuery, []);
            if (heatmapStateFromDB != null)
            {
                heatmapStateFromDB.PickupsAmount = heatmapState.PickupsAmount;
                heatmapStateFromDB.SumPickupRates = heatmapState.SumPickupRates;
                heatmapStateFromDB.AveragePickupRate = heatmapState.AveragePickupRate;
                heatmapStateFromDB.DeliveriesAmount = heatmapState.DeliveriesAmount;
                heatmapStateFromDB.SumDeliveryRates = heatmapState.SumDeliveryRates;
                heatmapStateFromDB.AverageDeliveryRate = heatmapState.AverageDeliveryRate;
                heatmapStateFromDB.Ranq = heatmapState.Ranq;

                await Repository.SaveAsync(heatmapStateFromDB);
            }
        }

        private void CalculateHeatmapStateValues(HeatmapStateDto heatmapState, List<ImportLoadDto> importLoads, bool isPickup)
        {
            decimal sumRates = 0;
            double sumMiles = 0;

            if (isPickup)
            {
                if (importLoads.Count() == 0)
                {
                    heatmapState.PickupsAmount = 0;
                    heatmapState.SumPickupRates = 0;
                    heatmapState.AveragePickupRate = 0;
                    return;
                }

                heatmapState.PickupsAmount = importLoads.Count();

                foreach (var il in importLoads)
                {
                    sumRates += il.Rate;
                    sumMiles += il.Miles;
                }

                heatmapState.SumPickupRates = sumRates;
                heatmapState.AveragePickupRate = decimal.Round(sumRates / (decimal)sumMiles, 2);
            }
            else
            {
                if (importLoads.Count() == 0)
                {
                    heatmapState.DeliveriesAmount = 0;
                    heatmapState.SumDeliveryRates = 0;
                    heatmapState.AverageDeliveryRate = 0;
                    return;
                }

                heatmapState.DeliveriesAmount = importLoads.Count();

                foreach (var il in importLoads)
                {
                    sumRates += il.Rate;
                    sumMiles += il.Miles;
                }

                heatmapState.SumDeliveryRates = sumRates;
                heatmapState.AverageDeliveryRate = decimal.Round(sumRates / (decimal)sumMiles, 2);
            }
        }
    }
}
