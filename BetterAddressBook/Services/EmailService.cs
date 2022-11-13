using BetterAddressBook.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using NuGet.Frameworks;

namespace BetterAddressBook.Services;

public class EmailService : IEmailSender
{
    private readonly MailSettings _mailSettings;

    public EmailService(IOptions<MailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var emailSender = _mailSettings.Email;
        MimeMessage newEmail = new()
        {
            Sender = MailboxAddress.Parse(emailSender),
            Subject = subject
        };

        foreach (var emaiLAddress in email.Split(";"))
        {
            newEmail.To.Add(MailboxAddress.Parse(emaiLAddress));
        }

        BodyBuilder emailBody = new BodyBuilder()
        {
            HtmlBody = htmlMessage
        };

        newEmail.Body = emailBody.ToMessageBody();

        using SmtpClient smtpClient = new();

        try
        {
            var host = _mailSettings.EmailHost;
            var port = _mailSettings.EmailPort;
            var password = _mailSettings.EmailPassword;

            await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(emailSender, password);
            await smtpClient.SendAsync(newEmail);
            await smtpClient.DisconnectAsync(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }

    }
}