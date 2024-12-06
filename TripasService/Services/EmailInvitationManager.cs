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
                string subject = "A friend has invited you to a match!";
                string displayName = "Tripas Game Invitation";
                string emailBody = CreateEmailBodyInvitation(code);
                operationResult = EmailHelper.SendEmail(emailReceiver, subject, emailBody, displayName);
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
    }
}