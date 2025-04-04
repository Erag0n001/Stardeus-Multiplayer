using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Game;
using Game.Commands;
using HarmonyLib;
using KL.Clock;
using Multiplayer.Managers;
using Multiplayer.Network;
using Shared.Enums;
using Shared.Misc;
using Shared.PacketData;

namespace Multiplayer.Patches
{
    public static class CmdLoadGamePatch
    {
        public static bool IsFromServer = false;
        [HarmonyPatch(typeof(CmdLoadGame), nameof(CmdLoadGame.ExecuteAsync))]
        public static class ExecuteAsyncPatch 
        {
            public static void Postfix(CmdLoadGame __instance) 
            {
                if (!Main.isConnected)
                    return;

                if (IsFromServer)
                {
                    IsFromServer = false;
                    InputManager.StartBroadcastingMousePos();
                    return;
                }

                string path = (string)AccessTools.Field(typeof(CmdLoadGame), "saveFile").GetValue(__instance);
                byte[] data = File.ReadAllBytes(path + ".save");
                SaveFile file = new SaveFile()
                {
                    data = data,
                    name = A.S.SaveName
                };
                ListenerClient.Instance.EnqueueObject(PacketType.SaveFileSend, file);
            }
        }
    }
}
