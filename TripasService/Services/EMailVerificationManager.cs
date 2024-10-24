using DataBaseManager.Utils;
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

namespace TripasService.Services {
    public partial class TripasGameService : IEmailVerificationManager {

        private static Dictionary<string, string> verificationCodes = new Dictionary<string, string>();
        public int sendVerificationCode(string emailReceiver) {
            int operationResult = Constants.FAILED;
            string code = generateVerificationCode();
            if (verificationCodes.ContainsKey(emailReceiver)) {
                verificationCodes[emailReceiver] = code;  
            }
            else {
                verificationCodes.Add(emailReceiver, code);
                string emailSender = "servicetripas@gmail.com";
                string emailPassword = "fxllpkrxfgnzbpvy";
                string displayName = "Verification-Code Tripas Game";
                try {
                    string emailBody = generateEmailBody(code);
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
                    operationResult = Constants.SUCCSESS;
                }
                catch (Exception ex) {
                    Console.WriteLine("Unable to send the mail "+ex.ToString());
                }

            }
            return operationResult;

        }

        public bool verifyCode(string email, string code) {
            if (verificationCodes.TryGetValue(email, out string storedCode)) {
                return storedCode.Equals(code);
            } else {
                return false;
            }
        }
        private string generateVerificationCode() {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider()) {
                byte[] randomBytes = new byte[4];
                rng.GetBytes(randomBytes);
                int code = BitConverter.ToInt32(randomBytes, 0) % 1000000; // 6 digits
                code = Math.Abs(code);
                string codeString = code.ToString("D6");
                return codeString;
            }
        }

        private string generateEmailBody(string code) {
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
    }
}
