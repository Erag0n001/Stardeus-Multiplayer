using System;
using System.Collections.Generic;
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
            [HarmonyPrefix]
            public static bool Prefix(GameState __instance, Entity entity) 
            {
                if (IsServer)
                {
                    return true;
                }
                if (!Main.isHost)
                    return false;
                return true;
            }
        }

        [HarmonyPatch(typeof(GameState), nameof(GameState.RemoveEntity))]
        public static class RemoveEntityPatch
        {
            public static bool IsServer;
            [HarmonyPrefix]
            public static bool Prefix(GameState __instance, Entity entity)
            {
                if (IsServer)
                    return true;
                if (!Main.isHost)
                    return false;
                Type type = entity.GetType();
                byte[] data = MessagePackSerializer.Serialize(type, entity, CmdSaveGame.MsgPackOptions);
                NetworkedEntityWithComp entityN = new NetworkedEntityWithComp();
                entityN.id = entity.Id;
                entityN.type = type;
                ListenerClient.Instance.EnqueueObject(Shared.Enums.PacketType.BroadCastEntityDelete, entityN);
                return true;
            }
            [HarmonyPostfix]
            public static void Postfix() 
            {
                if (IsServer)
                {
                    IsServer = false;
                    return;
                }
            }
        }
    }
}
