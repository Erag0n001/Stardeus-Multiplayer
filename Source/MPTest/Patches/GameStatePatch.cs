using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Commands;
using Game.Data;
using HarmonyLib;
using MessagePack;
using Multiplayer.Data;
using Multiplayer.Managers;
using Multiplayer.Misc;
using Multiplayer.Network;

namespace Multiplayer.Patches
{
    public static class GameStatePatch
    {
        [HarmonyPatch(typeof(GameState), nameof(GameState.AddEntity))]
        public static class AddEntityPatch
        {
            public static bool IsServer;
            [HarmonyPostfix]
            public static void Postfix(GameState __instance, Entity entity)
            {
                Printer.Warn($"Created object with id {entity.Id}");
            }
        }

        [HarmonyPatch(typeof(GameState), nameof(GameState.RemoveEntity))]
        public static class RemoveEntityPatch
        {
            public static bool IsServer;
            [HarmonyPrefix]
            public static bool Prefix(GameState __instance, Entity entity)
            {
                Printer.Warn($"Deleting object with id {entity.Id}");
                Printer.Warn($"Stacktrace : {new StackTrace()}");
                return true;
            }
        }
    }
}
