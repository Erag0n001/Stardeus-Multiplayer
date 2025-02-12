//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using Game;
//using Game.Commands;
//using Game.Components;
//using Game.Data;
//using Game.Systems.Items;
//using HarmonyLib;
//using MessagePack;
//using Multiplayer.Data;
//using Multiplayer.Managers;
//using Multiplayer.Misc;
//using Multiplayer.Network;
//using Shared.Network;
//using UnityEngine;

//namespace Multiplayer.Patches
//{
//    public static class ObjsPatches
//    {
//        public static bool IsFromServer;
//        [HarmonyPatch(typeof(Objs), nameof(Objs.Create), new Type[] { typeof(Vector2), typeof(Def), typeof(bool) })]
//        public static class ObjsCreateDef
//        {

//            [HarmonyPrefix]
//            public static bool Prefix()
//            {
//                if (!Main.isHost && !IsFromServer)
//                    return false;
//                return true;
//            }

//            [HarmonyPostfix]
//            public static void Postfix(Obj __result)
//            {
//                if (!Main.isHost)
//                    return;
//                if (IsFromServer)
//                {
//                    IsFromServer = false;
//                    return;
//                }
//                ObjectManager.HandleObjectCreation(__result);
//            }
//        }
//        [HarmonyPatch(typeof(Objs), nameof(Objs.Create), new Type[] { typeof(Vector2), typeof(ObjData), typeof(bool) })]
//        public static class ObjsCreateData
//        {

//            [HarmonyPrefix]
//            public static bool Prefix()
//            {
//                if (!Main.isHost && !IsFromServer)
//                    return false;
//                return true;
//            }

//            [HarmonyPostfix]
//            public static void Postfix(Obj __result)
//            {
//                if (IsFromServer)
//                {
//                    IsFromServer = false;
//                    return;
//                }
//                ObjectManager.HandleObjectCreation(__result);
//            }
//        }
//    }
//}
