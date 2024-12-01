﻿using TripasService.Utils;
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
        public int SendVerificationCodeRegister(string email) {
            int operationResult = Constants.FAILED_OPERATION;
            string code = GenerateCode();
            if (verificationCodesCreateAccount.ContainsKey(email)) {
                verificationCodesCreateAccount[email] = code;  
            }
            else {
                verificationCodesCreateAccount.Add(email, code);
                StartVerificationCodeTimer(email);
                string emailSender = "servicetripas@gmail.com";
                string emailPassword = "fxllpkrxfgnzbpvy";
                string displayName = "Verification-Code Tripas Game";
                try {
                    string emailBody = this.EmailBodyRegister(code);
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(emailSender, displayName);
                    mailMessage.To.Add(email);

                    mailMessage.Subject = "Your verification code";
                    mailMessage.Body = emailBody;
                    mailMessage.IsBodyHtml = true;
                    
                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                    smtpClient.Credentials = new NetworkCredential(emailSender, emailPassword);
                    smtpClient.EnableSsl = true;

                    smtpClient.Send(mailMessage);
                    operationResult = Constants.SUCCESSFUL_OPERATION;
                }
                catch (SmtpException smtpException) {
                    Console.WriteLine("Unable to send the mail "+smtpException.ToString());
                }

            }
            return operationResult;
        }

        public bool VerifyCode(string email, string code) {
            bool result = false;
            if (verificationCodesCreateAccount.TryGetValue(email, out string storedCode)) {
                if (storedCode.Equals(code)) {
                    verificationCodesCreateAccount.Remove(email);
                    result = true; 
                }
            } 
            return result; 
        }

        private string EmailBodyRegister(string code) {
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

        private bool VerifyCodeUniqueness(string code) {
            return verificationCodesCreateAccount.ContainsValue(code);
        }

        private string GenerateCode() {
            string code;
            do {
                code = CodesGeneratorHelper.GenerateVerificationCode();
            } while (VerifyCodeUniqueness(code));
            return code;
        }

        private void StartVerificationCodeTimer(string email) {
            Task.Run(async () => {
                await Task.Delay(60000); 
                verificationCodesCreateAccount.Remove(email);
            });
        }

    }
}
