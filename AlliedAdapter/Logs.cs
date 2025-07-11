using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlliedAdapter.Helpers;
using static AlliedAdapter.Helpers.Constants;

namespace AlliedAdapter
{
    public static class Logs
    {
        public const string _LogsFilePath = @"C:\inetpub\wwwroot\CEM\Logs\";
        public static void WriteLogEntry(LogType logType, string terminalIP, string message, string methodName)
        {
            try
            {
                string ip = terminalIP ?? string.Empty;

                string directoryPath = _LogsFilePath;
                string ipDirectoryPath = Path.Combine(directoryPath, ip);

                Directory.CreateDirectory(ipDirectoryPath);

   
                string fileName = $"{GetDate()} - Logs.txt";
                string filePath = Path.Combine(ipDirectoryPath, fileName);
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\t[{ip}]\t{logType}\t{methodName}\t{message}";
                    writer.WriteLine(logEntry);
                }
            }
            catch
            {
            }
        }
      
        public static string GetDate()
        {
            return DateTime.Now.ToString("ddMMMyyyy");
        }


    }
}
