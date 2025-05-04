using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using KL.Utils;
using Multiplayer.Misc;
using UnityEngine;

namespace Multiplayer.Patches
{
    public static class UnityLoggerPatch
    {
        [HarmonyPatch(typeof(Debug), nameof(Debug.Log), new Type[] { typeof(object) })]
        public static class LogPatch
        {
            [HarmonyPostfix]
            public static void Postfix(object message)
            {
                Printer.WriteToMultiplayerLog(message.ToString());
            }
        }
        [HarmonyPatch(typeof(Debug), nameof(Debug.LogWarning), new Type[] { typeof(object) })]
        public static class WarnPatch
        {
            [HarmonyPostfix]
            public static void Postfix(object message)
            {
                Printer.WriteToMultiplayerLog(message.ToString());
            }
        }
        [HarmonyPatch(typeof(Debug), nameof(Debug.LogError), new Type[] { typeof(object) })]
        public static class ErrPatch
        {
            [HarmonyPostfix]
            public static void Postfix(object message)
            {
                Printer.WriteToMultiplayerLog(message.ToString());
            }
        }
    }
}
