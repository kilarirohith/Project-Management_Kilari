namespace server.DTOs
{
    public class UserSettingsDto
    {
        public bool DarkMode { get; set; }
        public bool EmailNotifications { get; set; }
        public bool SmsNotifications { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string DefaultProjectView { get; set; } = "kanban";
        public int SessionTimeoutMinutes { get; set; } = 30;
    }
}
