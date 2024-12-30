using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using TripasService.Utils;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TripasService.Logic;

public static class EmailHelper {
    private const string SmtpServer = "smtp.gmail.com";
    private static string SmtpPort => Environment.GetEnvironmentVariable("PORT");
    private static string EmailSender => Environment.GetEnvironmentVariable("EMAIL_SENDER");
    private static string EmailPassword => Environment.GetEnvironmentVariable("EMAIL_PASSWORD");

    public static int SendEmail(string recipientEmail, string subject, string emailBody, string displayName) {
        LoggerManager logger = new LoggerManager(typeof(EmailHelper));
        int operationResult = Constants.FAILED_OPERATION;
        MailMessage mailMessage = CreateMailMessage(recipientEmail, subject, emailBody, displayName);
        SmtpClient smtpClient = CreateSmtpClient();
        try { 
            smtpClient.Send(mailMessage);
            operationResult = Constants.SUCCESSFUL_OPERATION;
        } catch (SmtpException smtpException) {
            logger.LogError($"Email sending failed: {smtpException.Message}", smtpException);
        }
        return operationResult;
    }

    private static MailMessage CreateMailMessage(string recipientEmail, string subject, string emailBody, string displayName) {
        MailMessage mailMessage = new MailMessage {
            From = new MailAddress(EmailSender, displayName),
            Subject = subject,
            Body = emailBody,
            IsBodyHtml = true
        };
        mailMessage.To.Add(recipientEmail);
        return mailMessage;
    }

    private static SmtpClient CreateSmtpClient() {
        int SmptServer = int.Parse(SmtpPort);

        SmtpClient smtpClient = new SmtpClient(SmtpServer, SmptServer) {
            Credentials = new NetworkCredential(EmailSender, EmailPassword),
            EnableSsl = true
        };
        return smtpClient;
    }

}