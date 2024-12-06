using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using TripasService.Services;
using TripasService.Utils;

namespace tripasHost {
    public class Program {
        static void Main(string[] args) {
            using (ServiceHost host = new ServiceHost(typeof(TripasService.Services.TripasGameService))) {
                host.Open();
                Console.WriteLine("Service connected");
                Console.ReadLine();
                //Comentario para subir cambios
            }
        }
    }
 }
