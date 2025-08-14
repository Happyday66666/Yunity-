using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Yunity.Services;

namespace WebPWrecover.Services;

public class EmailSender : IEmailSender
{
    private readonly ILogger _logger;

    public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor,
                       ILogger<EmailSender> logger)
    {
        Options = optionsAccessor.Value;
        _logger = logger;
    }

    public AuthMessageSenderOptions Options { get; } //Set with Secret Manager.

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        if (string.IsNullOrEmpty(Options.SmtpHost) || string.IsNullOrEmpty(Options.SmtpUsername) || string.IsNullOrEmpty(Options.SmtpPassword))
        {
            throw new Exception("SMTP settings are missing.");
        }
        Options.SmtpPassword = "bhdxsggwddwrshag";
        await Execute(Options.SmtpHost, Options.SmtpPort, Options.SmtpUsername, Options.SmtpPassword, subject, message, toEmail);
    }

    public async Task Execute(string smtpHost, int smtpPort, string smtpUsername, string smtpPassword, string subject, string message, string toEmail)
    {
        try
        {
            var smtpClient = new SmtpClient(smtpHost)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                 From = new MailAddress(smtpUsername, "Yunity 雲社區"), // ✅ 中文化發件人名稱
                //From = new MailAddress(smtpUsername, "Password Recovery"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);

            _logger.LogInformation($"Email to {toEmail} sent successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to send email to {toEmail}: {ex.Message}");
        }
    }
}
