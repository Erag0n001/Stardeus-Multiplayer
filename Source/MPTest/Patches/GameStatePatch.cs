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
                    return true;
                if (!Main.isHost)
                    return false;
                Type type = entity.GetType();
                byte[] data = MessagePackSerializer.Serialize(type, entity, CmdSaveGame.MsgPackOptions);
                NetworkedEntity entityN = new NetworkedEntity();
                entityN.data = data;
                entityN.type = type;
                ObjectWithCompsUtils.GetComps(entity, entityN);
                ListenerClient.Instance.EnqueueObject(Shared.Enums.PacketType.BroadCastNewEntity, entityN);
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
                NetworkedEntity entityN = new NetworkedEntity();
                entityN.data = data;
                entityN.type = type;
                ListenerClient.Instance.EnqueueObject(Shared.Enums.PacketType.BroadCastEntityDelete, entityN);
                return true;
            }
        }
    }
}
