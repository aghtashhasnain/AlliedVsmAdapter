using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace AlliedService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            ServiceProcessInstaller processInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            // Set the service account to user
            processInstaller.Account = ServiceAccount.User;
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            // Add installers
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}