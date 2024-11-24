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
    public class EmailInvitationManager : IEmailInvitationManager {

        private static Dictionary<string, DateTime> lastSentInvitationTime = new Dictionary<string, DateTime>();
        public int SendInvitation(string username, string code) {
            int operationResult = Constants.FAILED_OPERATION;
            string emailReceiver = UserDAO.GetMailByUsername(username);

            if (!string.IsNullOrEmpty(emailReceiver)) {
                if (CanSendInvitation(username)) {
                    string emailSender = "servicetripas@gmail.com";
                    string emailPassword = "fxllpkrxfgnzbpvy";
                    string displayName = "Tripas Game Invitation";

                    try {
                        string emailBody = this.emailBodyInvitation(code);
                        MailMessage mailMessage = new MailMessage();
                        mailMessage.From = new MailAddress(emailSender, displayName);
                        mailMessage.To.Add(emailReceiver);

                        mailMessage.Subject = "A friend has invited your to a match!";
                        mailMessage.Body = emailBody;
                        mailMessage.IsBodyHtml = true;

                        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                        smtpClient.Credentials = new NetworkCredential(emailSender, emailPassword);
                        smtpClient.EnableSsl = true;

                        smtpClient.Send(mailMessage);
                        operationResult = Constants.SUCCESSFUL_OPERATION;

                        lastSentInvitationTime[username] = DateTime.Now;
                    } catch (SmtpException smtpException) {
                        Console.WriteLine($" An SMTPException for {username} invitation. {smtpException.Message}");
                    }
                }
            }
            return operationResult;
        }

        private bool CanSendInvitation(string username) {
            bool result = true; 

            if (lastSentInvitationTime.ContainsKey(username)) {
                var lastSentTime = lastSentInvitationTime[username];
                if ((DateTime.Now - lastSentTime).TotalSeconds < 20) {
                    result = false; 
                }
            }
            return result;
        }

        private string emailBodyInvitation(string code) {
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
