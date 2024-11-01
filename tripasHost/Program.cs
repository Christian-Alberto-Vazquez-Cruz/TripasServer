using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TripasService.Services;
using TripasService.Utils;

namespace tripasHost {
    internal class Program {
        static void Main(string[] args) {
            using (ServiceHost host = new ServiceHost(typeof(TripasService.Services.TripasGameService))) {
                host.Open();
                Console.WriteLine("Server is running");
                TripasGameService service = new TripasGameService();
                string emailTest = "teemotatewaki@hotmail.com";
                int result = service.sendVerificationCodeRegister(emailTest);
                if (result == Constants.SUCCESSFUL_OPERATION) {
                    Console.WriteLine("Successfuly sent the verification code");
                    // Simular entrada de usuario con el código generado
                    Console.Write("Enter the verification code: ");
                    string userInputCode = Console.ReadLine();

                    // Verificar el código ingresado con el método verifyCode
                    bool isCodeValid = service.verifyCode(emailTest, userInputCode);
                    if (isCodeValid) {
                        Console.WriteLine("The verification code is valid!");
                    }
                    else {
                        Console.WriteLine("The verification code is invalid!");
                    }
                    Console.ReadLine();
                } else {
                    Console.WriteLine("Failed to send verification code.");
                }
                Console.ReadLine() ;

            }
        }
    }
}
