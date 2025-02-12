//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using Game;
//using Game.Commands;
//using Game.Components;
//using Game.Data;
//using Game.Systems.AI;
//using HarmonyLib;
//using MessagePack;
//using MPTest.Data;
//using MPTest.Misc;
//using MPTest.Network;

//namespace MPTest.Patches
//{
//    public static class AiSysPatch
//    {
//        public static bool IsFromServer;
//        private static readonly MethodInfo Set_State;
//        static AiSysPatch() 
//        {
//            Set_State = AccessTools.PropertySetter(typeof(AIGoal), "State");
//        }
//        [HarmonyPatch(typeof(AISys), "Tick")]
//        public static class ProcessGoalPreAssignedPatch 
//        {
//            [HarmonyPrefix]
//            public static bool Prefix(bool __result) 
//            {
//                __result = true;
//                if (IsFromServer)
//                    return true;
//                if (!Main.isHost)
//                    return false;
//                Printer.Error("", Shared.Enums.Verbose.StackTrace);
//                return true;
//            }

//            [HarmonyPostfix]
//            public static void Postfix(bool __result, AIAgentComp agent) 
//            {
//                if (!__result)
//                    return;
//                if (IsFromServer) 
//                {
//                    IsFromServer = false;
//                    return;
//                }
//                if (!Main.isHost)
//                    return;
//                Set_State.Invoke(agent.Goal, new object[] { AIGoalState.Available });
//                NetworkedAIGoal goalN = new NetworkedAIGoal();
//                AIGoalData.TrySerialize(agent.Goal, out AIGoalData data);
//                goalN.goal = MessagePackSerializer.Serialize(data);
//                Printer.Warn($"Agent was {agent?.ToString() ?? "null"}");
//                Printer.Error($"Job packet left with {data.Id} job id, {data.TargetId} target id, {data.AgentId} agent id");
//                ListenerClient.Instance.EnqueueObject("BroadCastNewAIGoal", goalN);
//            }
//        }
//    }
//}
