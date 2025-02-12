using System;
using System.Collections.Generic;
using Game;
using Game.Commands;
using Game.Components;
using Game.Constants;
using Game.Data;
using MessagePack;
using Multiplayer.Data;
using Multiplayer.Misc;
using Multiplayer.Network;
using Multiplayer.Patches;
using Shared.Enums;
using Shared.Misc;
using UnityEngine;

namespace Multiplayer.Managers
{
    public static class EntityManager
    {

    }
    public static class EntityHandler
    {
        [PacketHandler(PacketType.BroadCastNewEntity)]
        public static void SpawnEntityFromPacket(byte[] packet)
        {
            NetworkedObjectWithComp entityN = Serializer.PacketToObject<NetworkedObjectWithComp>(packet);
            if (!entityN.type.IsSubclassOf(typeof(Entity)))
            {
                Printer.Error($"Tried deserializing a non-whitelisted type of {entityN.type.Name} while spawning an Entity. " +
                    $"This is not allowed, you should only send childs of {typeof(Entity).Name} for spawning entities. skipping...");
                return;
            }
            Entity entity = (Entity)MessagePackSerializer.Deserialize(entityN.type, entityN.data, CmdSaveGame.MsgPackOptions);
            entity = Misc.EntityWithCompsUtils.Initialize(entity, entityN); //Handles the initialzation of the entity from the networked version
            GameStatePatch.AddEntityPatch.IsServer = true;
            Printer.Warn($"Spawning entity {entity}");
            A.S.AddEntity(entity);
        }

        [PacketHandler(PacketType.BroadCastEntityDelete)]
        public static void DeleteItemFromPacket(byte[] packet)
        {
            NetworkedObjectWithComp entityN = Serializer.PacketToObject<NetworkedObjectWithComp>(packet);
            Entity entity = A.S.FindEntity<Entity>((int)entityN.id);
            Printer.Warn($"Removing entity {entity}");
            GameStatePatch.RemoveEntityPatch.IsServer = true;
            A.S.RemoveEntity(entity);
        }
    }
}
