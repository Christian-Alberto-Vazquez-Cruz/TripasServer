using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Utils {
    public static class CodesGeneratorHelper {
        public static string GenerateVerificationCode() {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider()) {
                byte[] randomBytes = new byte[4];
                rng.GetBytes(randomBytes);
                int code = BitConverter.ToInt32(randomBytes, 0) % 1000000; 
                code = Math.Abs(code);
                string codeString = code.ToString("D6");
                return codeString;
            }
        }
    }
}
