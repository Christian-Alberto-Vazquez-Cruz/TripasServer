using TripasService.Contracts;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System;
using TripasService.Utils;
using DataBaseManager.DAO;

namespace TripasService.Services {
    public partial class TripasGameService : IPasswordRecoveryManager {
        private static Dictionary<string, string> recoveryCodes = new Dictionary<string, string>();

        public int SendRecoveryCode(string emailReceiver) {
            if (!verifyEmailRegistration(emailReceiver)) {
                return Constants.NO_MATCHES;
            }

            string code = CodesGeneratorHelper.GenerateVerificationCode();
            recoveryCodes[emailReceiver] = code;  
            StartRecoveryCodeTimer(emailReceiver);

            string emailSender = "servicetripas@gmail.com";
            string emailPassword = "fxllpkrxfgnzbpvy";
            string displayName = "Password Recovery - Tripas Game";
            try {
                string emailBody = EmailBodyRecovery(code);
                MailMessage mailMessage = new MailMessage {
                    From = new MailAddress(emailSender, displayName),
                    Subject = "Your password recovery code",
                    Body = emailBody,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(emailReceiver);

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587) {
                    Credentials = new NetworkCredential(emailSender, emailPassword),
                    EnableSsl = true
                };

                smtpClient.Send(mailMessage);
                return Constants.SUCCESS;
            }
            catch (SmtpException smtpException) {
                Console.WriteLine("Unable to send the mail: " + smtpException.ToString());
                return Constants.FAILED;
            }
        }

        public bool VerifyRecoveryCode(string email, string code) {
            if (recoveryCodes.TryGetValue(email, out string storedCode)) {
                if (storedCode.Equals(code)) {
                    recoveryCodes.Remove(email);
                    return true;
                }
            }
            return false;
        }

        public int UpdatePassword(string email, string newPassword) {
            int result = UserDAO.updateLoginPasswordDAO(email, newPassword);   
            return result; 
        }

        private string EmailBodyRecovery(string code) {
            return $@"
                <html>
                <body>
                    <h2>Password Recovery - Tripas Game</h2>
                    <p>Hello,</p>
                    <p>To recover your password, please use the following code:</p>
                    <h3 style='color:blue;'>{code}</h3>
                    <p>If you did not request this code, please ignore this email.</p>
                    <br>
                    <p>Best regards,</p>
                    <p><strong>Tripas Game Team</strong></p>
                </body>
                </html>";
        }
        private void StartRecoveryCodeTimer(string email) {
            Task.Run(async () => {
                await Task.Delay(60000);
                recoveryCodes.Remove(email);
                Console.WriteLine($"El código de recuperación para {email} ha sido eliminado después de 60 segundos.");
            });
        }

        private bool verifyEmailRegistration(string email) {
            bool result = false;
            if (UserDAO.isEmailRegisteredDAO(email)) {
                result = true;
            }
            return result;
        }

    }
}