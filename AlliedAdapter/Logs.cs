using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlliedAdapter
{
    public static class Logs
    {
        public const string _LogsFilePath = @"C:\inetpub\wwwroot\CEM\Logs\";

        public static void WriteLogEntry(string messageType, string TerminalIP, string message, string methodName)
        {
            try
            {
                string IP = "";
                try
                {
                    IP = TerminalIP;
                }
                catch
                {
                    IP = "";
                }
                //Check if Directory Exists, If not then create otherwise go ahead
                string directoryPathName = _LogsFilePath;//System.Configuration.ConfigurationManager.AppSettings["LogsPath"].ToString();



                bool directoryExists = Directory.Exists(directoryPathName);
                if (!directoryExists)
                {
                    Directory.CreateDirectory(directoryPathName);
                }

                string finalDirectoryPath = directoryPathName + IP;


                //Create IP wise Logs by making Directory named as IP value.
                //Check if directory of this IP's Name already exists or make new one.
                directoryExists = Directory.Exists(finalDirectoryPath);
                if (!directoryExists)
                {
                    Directory.CreateDirectory(finalDirectoryPath);
                }


                //Create Log File with name as Current Date
                //Check if File Exists into the directory, If yes then write a message 
                string filePathName = finalDirectoryPath + @"\" + GetDate() + " - Logs.txt";


                //Write an Entry into File         
                using (StreamWriter writer = new StreamWriter(filePathName, true))
                {
                    if (messageType.Contains("\n"))
                    {
                        writer.WriteLine("\n\n\n\n\n");
                        return;
                    }
                    
                    writer.WriteLine(DateTime.Now + "\t\t[" + IP + "]\t\t" + messageType + "\t\t" + methodName + "\t\t" + message);
                }

            }
            catch (Exception ex)
            {

            }



        }//end of method

        public static string GetDate()
        {
            return DateTime.Now.ToString("ddMMMyyyy");
        }

    }
}
