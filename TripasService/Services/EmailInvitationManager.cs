using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TripasService.Contracts;
using DataBaseManager.DAO;
using DataBaseManager.Utils;

namespace TripasService.Services {
    public partial class TripasGameService : IEmailInvitationManager {
        public int SendInvitation(string username, string code) {
            int operationResult = Constants.FAILED_OPERATION;
            string emailReceiver = UserDAO.GetMailByUsername(username);

            if (emailReceiver != Constants.NO_MATCHES_STRING) {
                string emailSender = "servicetripas@gmail.com";
                string emailPassword = "fxllpkrxfgnzbpvy";
                string displayName = "Tripas Game Invitation";

                try {
                    string emailBody = this.EmailBodyInvitation(code);
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(emailSender, displayName);
                    mailMessage.To.Add(emailReceiver);

                    mailMessage.Subject = "A friend has invited you to a match!";
                    mailMessage.Body = emailBody;
                    mailMessage.IsBodyHtml = true;

                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                    smtpClient.Credentials = new NetworkCredential(emailSender, emailPassword);
                    smtpClient.EnableSsl = true;

                    smtpClient.Send(mailMessage);
                    operationResult = Constants.SUCCESSFUL_OPERATION;
                } catch (SmtpException smtpException) {
                    Console.WriteLine($"An SMTPException for {username} invitation. {smtpException.Message}");
                }
            }
            return operationResult;
        }

        private string EmailBodyInvitation(string code) {
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
    }
}
