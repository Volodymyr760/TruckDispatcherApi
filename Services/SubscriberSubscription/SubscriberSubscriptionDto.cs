namespace TruckDispatcherApi.Services
{
    public class SubscriberSubscriptionDto
    {
        public required string SubscriberId { get; set; }

        public required string SubscriptionId { get; set; }

        public required SubscriberDto Subscriber { get; set; }

        public required SubscriptionDto Subscription { get; set; }
    }
}
