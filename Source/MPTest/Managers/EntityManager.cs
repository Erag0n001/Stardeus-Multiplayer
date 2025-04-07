using System;
using System.Threading;
using System.Threading.Tasks;
using Game;
using Game.Commands;
using Game.Data;
using MessagePack;
using Multiplayer.Data;
using Multiplayer.Misc;
using Multiplayer.Network;
using Multiplayer.Patches;
using Shared.Enums;
using Shared.Misc;
using static UnityEngine.EventSystems.EventTrigger;

namespace Multiplayer.Managers
{
    public static class EntityManager
    {
        public static void BroadcastEntity(Entity entity)
        {
            Task.Run(() =>
            {
                Thread.Sleep(2000);
                Type type = entity.GetType();
                byte[] data = MessagePackSerializer.Serialize(type, entity, CmdSaveGame.MsgPackOptions);

                NetworkedEntityWithComp entityN =
                    (entity is Obj) ? GetNetworkedObj(entity as Obj) :
                    (entity is Being) ? GetBeing(entity as Being) :
                    new NetworkedEntityWithComp();

                entityN.data = data;
                entityN.type = type;
                entityN.x = entity.Position.x;
                entityN.y = entity.Position.y;
                EntityWithCompsUtils.GetComps(entity, entityN);
                if (entity is Obj)
                    ListenerClient.Instance.EnqueueObject(PacketType.BroadCastNewObj, entityN);
                else if (entity is Being)
                    ;//Do nothing for now
                else
                    ListenerClient.Instance.EnqueueObject(PacketType.BroadCastNewEntity, entityN);
            });
        }
        private static NetworkedObj GetNetworkedObj(Obj obj) 
        {
            NetworkedObj objN = new NetworkedObj();
            objN.data = MessagePackSerializer.Serialize(typeof(Obj), obj, CmdSaveGame.MsgPackOptions);
            objN.color = obj.Data != null ? NetworkedColor.FromColor(obj.Data.Color) : null;
            objN.quality = obj.Data?.Quality;
            return objN;
        }
        private static NetworkedEntityWithComp GetBeing(Being being) 
        {
            return new NetworkedEntityWithComp();
        }
        public static void RemoveTile(Tile tile)
        {
            tile.S.Map.Grids[tile.Definition.LayerId].Set(tile.PosIdx, null);
            A.S.RemoveEntity(tile);
        }
    }
    public static class EntityHandler
    {
        [PacketHandler(PacketType.BroadCastNewEntity)]
        public static void BroadCastNewEntity(byte[] packet)
        {
            NetworkedEntityWithComp entityN = Serializer.PacketToObject<NetworkedEntityWithComp>(packet);
            if (!entityN.type.IsSubclassOf(typeof(Entity)))
            {
                Printer.Error($"Tried deserializing a non-whitelisted type of {entityN.type.Name} while spawning an Entity. " +
                    $"This is not allowed, you should only send childs of {typeof(Entity).Name} for spawning entities. skipping...");
                return;
            }
            EntityWithCompsUtils.Initialize(entityN); //Handles the initialzation of the entity from the networked version
        }

        [PacketHandler(PacketType.BroadCastEntityDelete)]
        public static void BroadCastEntityDelete(byte[] packet)
        {
            NetworkedEntityWithComp entityN = Serializer.PacketToObject<NetworkedEntityWithComp>(packet);
            Entity entity = A.S.FindEntity<Entity>((int)entityN.id);
            Printer.Warn($"Removing entity {entity}");
            //GameStatePatch.RemoveEntityPatch.IsServer = true;
            if (entity is Tile)
                EntityManager.RemoveTile(entity as Tile);
            if (entity is Obj)
                A.S.Objs.Destroy(entity as Obj, true);
        }
        [PacketHandler(PacketType.BroadCastNewObj)]
        public static void BroadCastNewObj(byte[] packet) 
        {
            NetworkedObj objN = Serializer.PacketToObject<NetworkedObj>(packet);
            EntityWithCompsUtils.Initialize(objN);
        }
    }
}
