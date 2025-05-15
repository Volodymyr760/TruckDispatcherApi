using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruckDispatcherApi.Library;
using TruckDispatcherApi.Services;

namespace TruckDispatcherApi.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Admin, Carrier, Broker")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class HeatmapController(ICityService cityService,
        IHeatmapService heatmapService,
        IHeatmapStateService heatmapStateService,
        IImportLoadService importLoadService) : AppControllerBase
    {
        private readonly ICityService cityService = cityService;
        private readonly IHeatmapService heatmapService = heatmapService;
        private readonly IHeatmapStateService heatmapStateService = heatmapStateService;
        private readonly IImportLoadService importLoadService = importLoadService;

        [HttpPost]
        public async Task<IActionResult> SearchAsync([FromBody] HeatmapSearchParams<HeatmapDto> searchParams) =>
            Ok(await heatmapService.GetAsync(searchParams));

        /// <summary>
        /// отримати з БД ImportLoads з відповідними обмеженнями по даті (початок і кінець поточної доби) і обладнанню
        /// зберегти Heatmap i HeatmapStates об’єкти в бд
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateAsync()
        {
            // Reset all HeatmapStates
            await heatmapStateService.ResetAsync();

            var equipments = new List<Equipment>() { Equipment.Flatbed, Equipment.Reefer, Equipment.Van };
            var dayTypes = new List<string>() { "Today", "Tomorrow" };

            //var todayStartDay = new DateTime(2025, 3, 31, 0, 0, 0);
            var todayStartDay = DateTime.UtcNow.Date;
            var todayEndDay = todayStartDay.AddDays(1).AddTicks(-1);
            var tomorrowStartDay = todayStartDay.AddDays(1);
            var tomorrowEndDay = todayEndDay.AddDays(1);

            foreach (var equipment in equipments)
            {
                foreach (var day in dayTypes)
                {
                    var heatmap = (await heatmapService.GetAsync(new HeatmapSearchParams<HeatmapDto>() { 
                         PageSize = 1,
                         CurrentPage = 1,
                         SearchCriteria = "",
                         UserId = "",
                         SortField = "",
                         Order = OrderType.Ascending,
                         IncludeNavProperties = true,
                         ItemList = [],
                         PageCount = 0,
                         TotalItemsCount = 0,
                         DayType = day,
                         Equipment = equipment
                    })).ItemList.ToList()[0];
                    List<ImportLoadDto> importLoads = [];
                    if (day == "Today")
                        importLoads = await importLoadService.GetAsync(todayStartDay, todayEndDay, equipment);
                    else
                        importLoads = await importLoadService.GetAsync(tomorrowStartDay, tomorrowEndDay, equipment);
                    heatmapStateService.GenerateHeatmapStatesAsync(heatmap, importLoads);
                    await heatmapService.ChangeUpdatedATAsync(heatmap);
                    foreach (var heatmapState in heatmap.HeatmapStates)
                        await heatmapStateService.SaveHeatmapStateAsync(heatmapState);
                }
            }

            return Ok();
        }
    }
}
