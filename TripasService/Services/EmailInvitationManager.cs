using System;
using System.Net;
using System.Net.Mail;
using TripasService.Contracts;
using DataBaseManager.DAO;
using DataBaseManager.Utils;

namespace TripasService.Services {

    public partial class TripasGameService : IEmailInvitationManager {

        public int SendInvitation(string username, string code) {
            int operationResult = Constants.FAILED_OPERATION;
            string emailReceiver = UserDAO.GetMailByUsername(username);
            if (emailReceiver != Constants.NO_MATCHES_STRING) {
                operationResult = SendEmailInvitation(emailReceiver, code);
            }
            return operationResult;
        }

        private int SendEmailInvitation(string emailReceiver, string code) {
            LoggerManager logger = new LoggerManager(this.GetType());
            int operationResult = Constants.FAILED_OPERATION;
            string emailSender = "servicetripas@gmail.com";
            string emailPassword = "fxllpkrxfgnzbpvy";
            string displayName = "Tripas Game Invitation";
            try {
                string emailBody = CreateEmailBodyInvitation(code);
                MailMessage mailMessage = CreateMailMessage(emailSender, displayName, emailReceiver, emailBody);
                SmtpClient smtpClient = CreateSmtpClient(emailSender, emailPassword);
                smtpClient.Send(mailMessage);
                operationResult = Constants.SUCCESSFUL_OPERATION;
            } catch (SmtpException smtpException) {
                logger.LogError(smtpException); 
                Console.WriteLine($"An exception involving EmailInvitationManager has ocurred {smtpException.ToString()}");
            }
            return operationResult;
        }

        private string CreateEmailBodyInvitation(string code) {
            return $@"
                <html>
                <body>
                    <h2>You're Invited to Tripas Game!</h2>
                    <p>Hello,</p>
                    <p>You have received an invitation to join Tripas Game. To get started, please use the code below:</p>
                    <h3 style='color:blue;'>{code}</h3>
                    <p>If you did not request this invitation, please ignore this email.</p>
                    <br>
                    <p>Best regards,</p>
                    <p><strong>Tripas Game Team</strong></p>
                </body>
                </html>";
        }

        private MailMessage CreateMailMessage(string emailSender, string displayName, string emailReceiver, string emailBody) {
            MailMessage mailMessage = new MailMessage {
                From = new MailAddress(emailSender, displayName),
                Subject = "A friend has invited you to a match!",
                Body = emailBody,
                IsBodyHtml = true
            };
            mailMessage.To.Add(emailReceiver);
            return mailMessage;
        }

        private SmtpClient CreateSmtpClient(string emailSender, string emailPassword) {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587) {
                Credentials = new NetworkCredential(emailSender, emailPassword),
                EnableSsl = true
            };
            return smtpClient;
        }
    }
}
