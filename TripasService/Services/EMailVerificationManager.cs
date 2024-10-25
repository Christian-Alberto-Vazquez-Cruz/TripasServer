using TripasService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TripasService.Contracts;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using DataBaseManager.DAO;

namespace TripasService.Services {
    public partial class TripasGameService : IEmailVerificationManager {

        private static Dictionary<string, string> verificationCodesCreateAccount = new Dictionary<string, string>();
        public int sendVerificationCodeRegister(string emailReceiver) {

            int operationResult = Constants.FAILED;
            string code = generateCode();
            if (verificationCodesCreateAccount.ContainsKey(emailReceiver)) {
                verificationCodesCreateAccount[emailReceiver] = code;  
            }
            else {
                verificationCodesCreateAccount.Add(emailReceiver, code);
                StartVerificationCodeTimer(emailReceiver);

                string emailSender = "servicetripas@gmail.com";
                string emailPassword = "fxllpkrxfgnzbpvy";
                string displayName = "Verification-Code Tripas Game";
                try {
                    string emailBody = this.emailBodyRegister(code);
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(emailSender, displayName);
                    mailMessage.To.Add(emailReceiver);

                    mailMessage.Subject = "Your verification code";
                    mailMessage.Body = emailBody;
                    mailMessage.IsBodyHtml = true;
                    
                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                    smtpClient.Credentials = new NetworkCredential(emailSender, emailPassword);
                    smtpClient.EnableSsl = true;

                    smtpClient.Send(mailMessage);
                    operationResult = Constants.SUCCESS;
                }
                catch (SmtpException smtpException) {
                    Console.WriteLine("Unable to send the mail "+smtpException.ToString());
                }

            }
            return operationResult;
        }

        public bool verifyCode(string email, string code) {
            bool result = false;
            if (verificationCodesCreateAccount.TryGetValue(email, out string storedCode)) {
                if (storedCode.Equals(code)) {
                    verificationCodesCreateAccount.Remove(email);
                    result = true; 
                }
            } 
            return result; 
        }

        private string emailBodyRegister(string code) {
            return $@"
                <html>
                <body>
                    <h2>Welcome to Tripas Game!</h2>
                    <p>Hello,</p>
                    <p>Thank you for registering with Tripas Game. To complete your registration, please use the verification code below:</p>
                    <h3 style='color:blue;'>{code}</h3>
                    <p>If you did not request this code, please ignore this email.</p>
                    <br>
                    <p>Best regards,</p>
                    <p><strong>Tripas Game Team</strong></p>
                </body>
                </html>";
        }

        private bool verifyCodeUniqueness(string code) {
            return verificationCodesCreateAccount.ContainsValue(code);
        }

        private string generateCode() {
            string code;
            do {
                code = CodesGeneratorHelper.GenerateVerificationCode();
            } while (verifyCodeUniqueness(code));
            return code;
        }

        private void StartVerificationCodeTimer(string email) {
            Task.Run(async () => {
                await Task.Delay(60000); 
                verificationCodesCreateAccount.Remove(email);
                Console.WriteLine($"El código de verificación para {email} ha sido eliminado después de 60 segundos.");
            });
        }

    }
}
