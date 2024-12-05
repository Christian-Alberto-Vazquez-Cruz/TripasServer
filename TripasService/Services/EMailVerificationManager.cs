using TripasService.Utils;
using System.Threading.Tasks;
using TripasService.Contracts;
using System.Collections.Generic;

namespace TripasService.Services {

    public partial class TripasGameService : IEmailVerificationManager {

        private static readonly Dictionary<string, string> _verificationCodesCreateAccount = new Dictionary<string, string>();

        public int SendVerificationCodeRegister(string email) {
            int operationResult = Constants.FAILED_OPERATION;
            string subject = "Your verification code";
            string displayName = "Verification-Code Tripas Game"; 
            string code = GenerateCode();
            StoreVerificationCode(email, code);
            string emailBody = CreateEmailBodyRegister(code);
            operationResult = EmailHelper.SendEmail(email, subject, emailBody, displayName);
            return operationResult;
        }

        private void StoreVerificationCode(string email, string code) {
            if (_verificationCodesCreateAccount.ContainsKey(email)) {
                _verificationCodesCreateAccount[email] = code;
            } else {
                _verificationCodesCreateAccount.Add(email, code);
                StartVerificationCodeTimer(email);
            }
        }

        private string CreateEmailBodyRegister(string code) {
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
            bool codeUniqueness = _verificationCodesCreateAccount.ContainsValue(code);
            return codeUniqueness;
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
                _verificationCodesCreateAccount.Remove(email);
            });
        }

        public bool VerifyCode(string email, string code) {
            bool verificationResult = false;
            if (_verificationCodesCreateAccount.TryGetValue(email, out string storedCode)) {
                if (storedCode.Equals(code)) {
                    _verificationCodesCreateAccount.Remove(email);
                    verificationResult = true;
                }
            }
            return verificationResult;
        }
    }
}
