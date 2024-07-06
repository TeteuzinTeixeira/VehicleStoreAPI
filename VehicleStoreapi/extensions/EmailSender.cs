using System.Net;
using System.Net.Mail;
using System.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using VehicleStoreapi.Database;

namespace VehicleStoreapi.extensions;

public class EmailSender : IEmailSender
{
    private readonly EmailSettings _emailSettings;

    public EmailSender(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var client = new SmtpClient(_emailSettings.SMTPServer, _emailSettings.SMTPPort)
        {
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_emailSettings.SMTPUsername, _emailSettings.SMTPPassword),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(email);

        return client.SendMailAsync(mailMessage);
    }
}
