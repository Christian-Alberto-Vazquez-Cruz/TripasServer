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
    private const int SmtpPort = 587;
    private const string EmailSender = "servicetripas@gmail.com";
    private const string EmailPassword = "fxllpkrxfgnzbpvy";

    public static int SendEmail(string recipientEmail, string subject, string emailBody, string displayName) {
        LoggerManager logger = new LoggerManager(typeof(EmailHelper));
        try {
            MailMessage mailMessage = new MailMessage {
                From = new MailAddress(EmailSender, displayName),
                Subject = subject,
                Body = emailBody,
                IsBodyHtml = true
            };
            mailMessage.To.Add(recipientEmail);

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587) {
                Credentials = new NetworkCredential(EmailSender, EmailPassword),
                EnableSsl = true
            };
            smtpClient.Send(mailMessage);
            return Constants.SUCCESSFUL_OPERATION;
        } catch (SmtpException smtpException) {
            logger.LogError($"Email sending failed: {smtpException.Message}", smtpException);
            return Constants.FAILED_OPERATION;
        }
    }
}