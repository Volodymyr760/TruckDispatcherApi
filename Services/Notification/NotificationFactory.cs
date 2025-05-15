namespace TruckDispatcherApi.Services
{
    public class NotificationFactory
    {
        public static NotificationDto GetNotification(string? senderAvatarUrl, string senderFullName,
            string message, string? callBackUrl, string recipientId, string recipientEmail) =>
            new()
            {
                SenderAvatarUrl = senderAvatarUrl,
                SenderFullName = senderFullName,
                Message = message,
                IsRead = false,
                CallBackUrl = callBackUrl,
                CreatedAt = DateTime.UtcNow,
                RecipientId = recipientId,
                RecipientEmail = recipientEmail
            };
    }
}
