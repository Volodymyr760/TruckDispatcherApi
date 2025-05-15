using AutoMapper;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Broker, BrokerDto>().ReverseMap();

            CreateMap<City, CityDto>().ReverseMap();

            CreateMap<Models.Client, ClientDto>().ReverseMap();

            CreateMap<Driver, DriverDto>().ReverseMap();
            CreateMap<Driver, SearchDriverDto>();

            CreateMap<Heatmap, HeatmapDto>().ReverseMap();
            CreateMap<HeatmapState, HeatmapStateDto>().ReverseMap();

            CreateMap<Image, ImageDto>().ReverseMap();

            CreateMap<Invoice, InvoiceDto>().ReverseMap();

            CreateMap<ImportLoad, ImportLoadDto>().ReverseMap();

            CreateMap<Load, LoadDto>().ReverseMap();

            CreateMap<MailTemplate, MailTemplateDto>().ReverseMap();

            CreateMap<Notification, NotificationDto>().ReverseMap();

            CreateMap<Pricepackage, PricepackageDto>().ReverseMap();

            CreateMap<Subscriber, SubscriberDto>().ReverseMap();

            CreateMap<SubscriberSubscription, SubscriberSubscriptionDto>().ReverseMap();

            CreateMap<Subscription, SubscriptionDto>().ReverseMap();

            CreateMap<Truck, TruckDto>().ReverseMap();

            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
