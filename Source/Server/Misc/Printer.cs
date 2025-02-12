using Shared.Enums;

namespace Server.Misc
{
    public static class Printer
    {
        private static readonly string LogMessage = "[MP]> ";
        public static void Log(object toLog, Verbose modifier = Verbose.Normal)
        {
            string messageToLog = string.Empty;

            Console.ForegroundColor = ConsoleColor.White;
            messageToLog += $"{HandleVerbose(modifier, toLog)}";
            Console.WriteLine(messageToLog);
            Console.ResetColor();
        }
        public static void Warn(object toLog, Verbose modifier = Verbose.Normal)
        {
            string messageToLog = string.Empty;

            Console.ForegroundColor = ConsoleColor.Yellow;
            messageToLog += $"{HandleVerbose(modifier, toLog)}";
            Console.WriteLine(messageToLog);
            Console.ResetColor();
        }
        public static void Error(object toLog, Verbose modifier = Verbose.Normal)
        {
            string messageToLog = string.Empty;

            Console.ForegroundColor = ConsoleColor.Red;
            messageToLog += $"{HandleVerbose(modifier, toLog)}";
            Console.WriteLine(messageToLog);
            Console.ResetColor();
        }

        private static string HandleVerbose(Verbose verbose, object toLog)
        {
            switch (verbose)
            {
                case Verbose.Normal:
                    return LogMessage + (toLog?.ToString() ?? "null");
                case Verbose.Verbose:
                    return string.Empty; //ToAddConfig
                case Verbose.StackTrace:
                    return LogMessage + $"\nStacktrace:\n{new System.Diagnostics.StackTrace().ToString()}\n";
                default:
                    return LogMessage + (toLog?.ToString() ?? "");
            }
        }
    }
}
