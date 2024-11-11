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
                Console.WriteLine("Service connected");
                Console.ReadLine();
            }
        }
    }
    }
