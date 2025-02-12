//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Game;
//using Game.Commands;
//using Game.Data;
//using Game.Systems.Items;
//using HarmonyLib;
//using MessagePack;
//using Multiplayer.Data;
//using Multiplayer.Managers;
//using Multiplayer.Misc;
//using Multiplayer.Network;
//using Shared.Enums;
//using UnityEngine;

//namespace Multiplayer.Patches
//{
//    public static class CmdPlaceTilesPatch
//    {
//        public static bool IsFromServer;

//        [HarmonyPrefix]
//        public static bool Prefix()
//        {
//            if (!Main.isHost && !IsFromServer)
//                return false;
//            return true;
//        }

//        [HarmonyPatch(typeof(CmdPlaceTile), nameof(CmdPlaceTile.PlaceTileWithChecks))]
//        public static class BuildTilePatch
//        {
//            [HarmonyPostfix]
//            public static void Postfix(Tile tile, bool instant)
//            {
//                HandleTileCreation(tile, instant);
//            }
//        }

//        static void HandleTileCreation(Tile tile, bool instant)
//        {
//            InputManager.StartBroadcastingMousePos();
//            if (!Main.isHost)
//                return;
//            if (IsFromServer)
//            {
//                IsFromServer = false;
//                return;
//            }
//            if (!The.GameIsInitialized) return;
//            if (tile != null)
//            {
//                NetworkedTile networkedObject = new NetworkedTile();
//                networkedObject.def = tile.Definition.Id;
//                networkedObject.x = tile.Position.x;
//                networkedObject.y = tile.Position.y;
//                networkedObject.instant = instant;
//                ObjectWithCompsUtils.GetComps(tile, networkedObject);
//                ListenerClient.Instance.EnqueueObject(PacketType.BroadCastNewTile, networkedObject);
//            }
//        }
//    }
//}
