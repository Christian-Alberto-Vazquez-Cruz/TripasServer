using System;
using TripasService.Utils;
using DataBaseManager.DAO;
using System.Threading.Tasks;
using TripasService.Contracts;
using System.Collections.Generic;

namespace TripasService.Services {
    public partial class TripasGameService : IPasswordRecoveryManager {

        private static readonly Dictionary<string, string> _recoveryCodes = new Dictionary<string, string>();

        public int SendRecoveryCode(string email) {
            int operationResult = UserDAO.IsEmailRegisteredDAO(email);
            if (operationResult != Constants.FOUND_MATCH) {
                return operationResult;
            }
            string code = GenerateAndStoreRecoveryCode(email);
            string subject = "Your password recovery code";
            string displayName = "Password Recovery - Tripas Game";
            string emailBody = CreateEmailBodyRecovery(code);
            operationResult = EmailHelper.SendEmail(email, subject, emailBody, displayName);
            return operationResult;
        }

        private string GenerateAndStoreRecoveryCode(string email) {
            string code = CodesGeneratorHelper.GenerateVerificationCode();
            _recoveryCodes[email] = code;
            StartRecoveryCodeTimer(email);
            return code;
        }

        public bool VerifyRecoveryCode(string email, string code) {
            if (_recoveryCodes.TryGetValue(email, out string storedCode)) {
                if (storedCode.Equals(code)) {
                    _recoveryCodes.Remove(email);
                    return true;
                }
            }
            return false;
        }

        public int UpdatePassword(string email, string newPassword) {
            int result = UserDAO.UpdateLoginPasswordDAO(email, newPassword);
            return result;
        }

        private string CreateEmailBodyRecovery(string code) {
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
                _recoveryCodes.Remove(email);
                Console.WriteLine($"El código de recuperación para {email} ha sido eliminado después de 60 segundos.");
            });
        }
    }
}