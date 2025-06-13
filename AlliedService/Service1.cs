using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Threading;

namespace AlliedService
{
    public partial class Service1 : ServiceBase
    {
        private System.Timers.Timer timer;

        public Service1()
        {
            InitializeComponent();
        }


        public void StartServiceManually()
        {
            OnStart(null);
        }
        protected override void OnStart(string[] args)
        {
            try
            {
                //WriteLogEntry("AlliedService is starting.");

                System.Diagnostics.Debugger.Launch();
                HitIt();
                timer = new System.Timers.Timer();
                timer.Interval = 20000;
                timer.Elapsed += Timer_Elapsed;
                timer.AutoReset = true;
                timer.Enabled = true;

                WriteLogEntry("Info", "Timer started executing every 10 seconds  AlliedService started successfully" + "AllFiles", "DecryptEmbossingFile");


            }
            catch (Exception ex)
            {


                WriteLogEntry("Error", ex.Message, "Error starting AlliedService:");
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                HitIt();
            }
            catch (Exception ex)
            {
                WriteLogEntry("Error", "Error in Timer_Elapsed: " + ex.Message, "Timer_Elapsed");
            }
        }

        private void HitIt()
        {
            try
            {
                string processName = "Embossing";
                var running = Process.GetProcessesByName(processName).Any(); 
                
                if (!running)
                {
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    string batFilePath = Path.Combine(basePath, "MonitorApp.bat");
                    WriteLogEntry("Info", "Bat File Path. : " + batFilePath, "HitIt");
                    if (File.Exists(batFilePath))
                    {
                        ProcessStartInfo psi = new ProcessStartInfo
                        {
                            FileName = batFilePath,
                            UseShellExecute = true, 
                            WindowStyle = ProcessWindowStyle.Normal
                        };

                        Process.Start(psi);
                        WriteLogEntry("Info", "Batch file started successfully.", "HitIt"); 
                    }
                    else
                    {
                        WriteLogEntry("Error", "Batch file not found.", "HitIt");
                    }
                }
                else
                {
                    WriteLogEntry("Info", "Console app already running.", "HitIt");
                }
            }
            catch (Exception ex)
            {
                WriteLogEntry("Error", "Error in HitIt(): " + ex.Message, "HitIt");
            }
        }



        protected override void OnStop()
        {
            try
            {
                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                    timer = null;
                }
                 
            }
            catch (Exception ex)
            {
            }
        }
        public static void WriteLogEntry(string messageType, string message, string methodName)
        {
            try
            {

                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string directoryPathName = Path.Combine(basePath, "Logs");

                // Ensure the Logs directory exists
                if (!Directory.Exists(directoryPathName))
                {
                    Directory.CreateDirectory(directoryPathName);
                }

                string finalDirectoryPath = directoryPathName;

                string filePathName = finalDirectoryPath + @"\" + GetDate() + " - Logs.txt";


                using (StreamWriter writer = new StreamWriter(filePathName, true))
                {
                    if (messageType.Contains("\n"))
                    {
                        writer.WriteLine("\n\n\n\n\n");
                        return;
                    }

                    writer.WriteLine(DateTime.Now + messageType + "\t\t" + methodName + "\t\t" + message);
                }

            }
            catch (Exception ex)
            {

            }

        }

        public static string GetDate()
        {
            return DateTime.Now.ToString("ddMMMyyyy");
        }

    }
}