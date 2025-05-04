using System.IO;
using System;
using KL.Utils;
using Shared.Enums;
using UnityEngine;
using Game;
using static Game.Systems.ParticlesSys;
using WebSocketSharp;

namespace Multiplayer.Misc
{
    public static class Printer
    {
        private static object printLock = new object();
        private static string LogFolder => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "..",
            "LocalLow",
            "Kodo Linija",
            "Stardeus",
            "MpLogs"
            );
        private static readonly string extension = ".log";
        private static readonly string logPath;
        static Printer() 
        {
            if(!Directory.Exists(LogFolder)) 
            {
                Directory.CreateDirectory(LogFolder);
            }
            var files = Directory.GetFiles(LogFolder);
            logPath = Path.Combine(LogFolder, $"{The.Platform.PlayerName ?? "Test"}{extension}");
            if(File.Exists(logPath))
                File.Delete(logPath);
            using (File.Create(logPath));
        }
        private static readonly string LogMessage = "[MP]> ";
        public static void Log(object toLog, Verbose modifier = Verbose.Normal)
        {
            lock (printLock)
            {
                string messageToLog = string.Empty;

                messageToLog += $"{HandleVerbose(modifier, toLog)}";
                D.Log(messageToLog);
            }
        }
        public static void Warn(object toLog, Verbose modifier = Verbose.Normal)
        {
            lock (printLock)
            {
                string messageToLog = $"{HandleVerbose(modifier, toLog)}";
                D.Warn(messageToLog);
            }
        }
        public static void Error(object toLog, Verbose modifier = Verbose.Normal)
        {
            lock (printLock)
            {
                string messageToLog = $"{HandleVerbose(modifier, toLog)}";
                D.Err(messageToLog);
            }
        }
        private static string HandleVerbose(Verbose verbose, object toLog)
        {
            switch (verbose)
            {
                case Verbose.Normal:
                    return LogMessage + (toLog?.ToString() ?? "null");
                case Verbose.Verbose:
                    return string.Empty; //ToAddModConfig
                case Verbose.StackTrace:
                    return LogMessage + $"\nStacktrace:\n{new System.Diagnostics.StackTrace().ToString()}\n";
                default:
                    return LogMessage + (toLog?.ToString() ?? "");
            }
        }

        public static void WriteToMultiplayerLog(string messageToLog)
        {
            File.AppendAllText(logPath, messageToLog + "\n");
        }
    }
}
