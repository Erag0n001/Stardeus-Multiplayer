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
                {
                    return true;
                }
                if (!Main.isHost)
                    return false;
                return true;
            }
            [HarmonyPostfix]
            public static void Postfix(GameState __instance, Entity entity) 
            {
                if (IsServer)
                {
                    IsServer = false;
                    return;
                }
                if (!Main.isHost)
                    return;
                Type type = entity.GetType();
                byte[] data = MessagePackSerializer.Serialize(type, entity, CmdSaveGame.MsgPackOptions);
                NetworkedObjectWithComp entityN = new NetworkedObjectWithComp();
                entityN.data = data;
                entityN.type = type;
                entityN.posIdx = entity.PosIdx;
                Misc.EntityWithCompsUtils.GetComps(entity, entityN);
                ListenerClient.Instance.EnqueueObject(Shared.Enums.PacketType.BroadCastNewEntity, entityN);
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
                NetworkedObjectWithComp entityN = new NetworkedObjectWithComp();
                entityN.id = entity.Id;
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
