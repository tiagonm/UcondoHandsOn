namespace AccountPlan.Application.Validators
{
    public class NotificationContext
    {
        private readonly List<string> _notifications = new();

        public IReadOnlyCollection<string> Notifications => _notifications;
        public bool HasNotifications => _notifications.Any();

        public void AddNotification(string message) => _notifications.Add(message);
        public void AddNotifications(IEnumerable<string> messages) => _notifications.AddRange(messages);
    }

}
