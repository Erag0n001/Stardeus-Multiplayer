using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Game;
using Game.Commands;
using Game.Components;
using Game.Data;
using Game.Systems.Items;
using HarmonyLib;
using KL.Utils;
using MessagePack;
using Multiplayer.Data;
using Multiplayer.Misc;
using Multiplayer.Network;
using Multiplayer.Patches;
using Shared.Enums;
using Shared.Misc;
using UnityEngine;
using static Game.Systems.ParticlesSys;
using static Game.UI.QuickSearch;

namespace Multiplayer.Managers
{
    public static class ObjectManager
    {
        public static void HandleObjectCreation(Obj __result)
        {
            //if (!The.GameIsInitialized) return;
            //if (__result != null)
            //{
            //    NetworkedObjectWithComp networkedObject = new NetworkedObjectWithComp();
            //    networkedObject.def = __result.Definition.Id;
            //    networkedObject.x = __result.Position.x;
            //    networkedObject.y = __result.Position.y;
            //    ObjectWithCompsUtils.GetComps(__result, networkedObject);
            //    List<byte[]> packets = Serializer.CreatePacketsFromObject(networkedObject, PacketType.BroadCastNewObject);
            //    int count = 0;
            //    foreach (byte[] packet in packets)
            //    {
            //        count += packet.Length;
            //    }
            //    ListenerClient.Instance.EnqueuePackets(packets);
            //}
        }

        public static void SpawnItem(NetworkedObjectWithComp objN)
        {
            //ObjsPatches.IsFromServer = true;
            //Obj obj = A.S.Objs.Create(new Vector2(objN.x, objN.y), The.Defs.Get(objN.def));
            //ObjectWithCompsUtils.SetComps(obj, objN);
        }

    }
    public static class ObjectHandler 
    {
        [PacketHandler(PacketType.BroadCastNewObject)]
        public static void SpawnItemFromPacket(byte[] packet)
        {
            NetworkedObjectWithComp data = Serializer.PacketToObject<NetworkedObjectWithComp>(packet);
            ObjectManager.SpawnItem(data);
        }
        [PacketHandler(PacketType.BroadCastNewTile)]
        public static void SpawnTileFromPacket(byte[] packet)
        {
            //CmdPlaceTilesPatch.IsFromServer = true;
            //NetworkedTile obj = Serializer.PacketToObject<NetworkedTile>(packet);
            //Tile tile = EntityUtils.CreatePrototype<Tile>(obj.def);
            //CmdPlaceTile cmd = new CmdPlaceTile(EntityUtils.ToPosSafe(new Vector2(obj.x, obj.y)), tile, false, obj.instant, true);
            //ObjectWithCompsUtils.SetComps(tile, obj);
            //A.S.CmdQ.Enqueue(cmd);
        }
    }
}
