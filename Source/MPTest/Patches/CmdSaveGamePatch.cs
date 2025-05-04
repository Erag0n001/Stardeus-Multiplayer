using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Game;
using Game.Commands;
using Game.Data;
using HarmonyLib;
using Mono.Cecil.Cil;
using Multiplayer.Misc;
using Multiplayer.Network;
using Shared.Enums;
using Shared.PacketData;

namespace Multiplayer.Patches
{
    public static class CmdSaveGamePatch
    {
        private static byte[] data;
        private static byte[] meta;
        [HarmonyPatch(typeof(CmdSaveGame), nameof(CmdSaveGame.SaveMeta))]
        public static class SaveMetaPatch 
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) 
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
                MethodInfo toCall = AccessTools.Method(typeof(SaveMetaPatch), "WriteBytes");
                for (int i = 0; i < codes.Count; i++)
                {
                    CodeInstruction code = codes[i];
                    if(code.opcode == OpCodes.Stloc_0) 
                    {
                        codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, toCall));
                        codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldarg_0));
                    }
                }
                return codes;
            }
            private static void WriteBytes(byte[] bytes)
            {
                if (!Main.isConnected)
                    return;
                meta = bytes;
            }
        }
        [HarmonyPatch(typeof(CmdSaveGame), "WriteSaveWithBackup")]
        public static class WriteSaveWithBackupPatch
        {
            [HarmonyPrefix]
            public static void Postfix(byte[] data)
            {
                if (!Main.isConnected)
                    return;
                CmdSaveGamePatch.data = data;
            }
        }
        [HarmonyPatch(typeof(CmdSaveGame), "SaveGame")]
        public static class SaveGamePatch 
        {
            public static void Postfix(CmdSaveGame __instance)
            {
                bool isExiting = (bool)AccessTools.Field(typeof(CmdSaveGame), "willExit").GetValue(__instance);
                if (Main.isConnected)
                {
                    Printer.Warn($"Sending save to server");
                    SaveFile save = new SaveFile();
                    save.name = The.Platform.PlayerName ?? "Test";
                    save.data = data;
                    save.meta = meta;
                    ListenerClient.Instance.EnqueueObject(PacketType.SaveFileSend, save);
                    if (isExiting)
                    {
                        ListenerClient.Instance.EnqueueObject(PacketType.DisconnectSafe, new object());
                        ListenerClient.Instance.disconnectFlagSmooth = true;
                    }
                }
            }
        }
    }
}
