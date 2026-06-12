using System;
using System.Diagnostics;
using System.Configuration;

namespace DVLD_DataAccess
{
    internal class clsDataAccessSettings
    {

        public static string ConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
       

        // Get Log Level Priority
        private static int _GetLogLevelPriority(string level)
        {
            switch (level)
            {
                case "Debug": return 1;
                case "Info": return 2;
                case "Warning": return 3;
                case "Error": return 4;
                default: return 4;
            }
        }

        private const string SourceName = "DVLD";
        private const string LogName = "Application";
        public static void LogEvent(string message, EventLogEntryType type)
        {
            // Get LogLevel from App.config (Default to Error if null)
            string configLogLevel = ConfigurationManager.AppSettings["LogLevel"] ?? "Error";

            int currentPriority = (type == EventLogEntryType.Error) ? 4 :
                                 (type == EventLogEntryType.Warning) ? 3 : 2;

            // Check if current event priority meets the config threshold
            if (currentPriority >= _GetLogLevelPriority(configLogLevel))
            {
                try
                {
                    if (!EventLog.SourceExists(SourceName))
                    {
                        EventLog.CreateEventSource(SourceName, LogName);
                    }
                    EventLog.WriteEntry(SourceName, message, type);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fialed :" + ex.Message);
                }
            }
        }


    }
}