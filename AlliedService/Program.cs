using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AlliedService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
            static void Main(string[] args)
            {
                if (Environment.UserInteractive)
                {
                    Service1 service = new Service1();
                    service.StartServiceManually();  // Simulate the service start
              
                }
                else
                {
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[] { new Service1() };
                    ServiceBase.Run(ServicesToRun);
                }
            }

      

    }
}
