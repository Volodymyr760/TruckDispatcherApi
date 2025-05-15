namespace TruckDispatcherApi.Models
{
    public class SubscriberSubscription
    {
        public required string SubscriberId { get; set; }

        public required string SubscriptionId { get; set; }

        public required Subscriber Subscriber { get; set; }

        public required Subscription Subscription { get; set; }
    }
}
