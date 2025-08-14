namespace Yunity.Services
{
    public class AuthMessageSenderOptions
    {
        public string? SendGridKey { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
    }
}

