//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Game.Components;
//using Game.Data;
//using HarmonyLib;
//using Multiplayer.Managers;
//using UnityEngine;
//using static UnityEngine.EventSystems.EventTrigger;

//namespace Multiplayer.Patches
//{
//    public static class ObjsPatch
//    {
//        //[HarmonyPatch(typeof(Objs), nameof(Objs.Create), new Type[] { typeof(Vector2), typeof(Def), typeof(bool) })]
//        //public static class CreatePatch 
//        //{
//        //    [HarmonyPostfix]
//        //    public static void Postfix(Obj __result)
//        //    {
//        //        if (!Main.isHost)
//        //            return;
//        //        EntityManager.BroadcastEntity(__result);
//        //    }
//        //}
//    }
//}
