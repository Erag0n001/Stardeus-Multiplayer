using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Game;
using Game.Systems.AI;
using HarmonyLib;
using MessagePack;
using Multiplayer.Data;
using Multiplayer.Misc;
using Multiplayer.Network;
using Shared.Enums;

namespace Multiplayer.Patches
{
    public static class AiAgentCompPatch
    {
        public static bool IsFromServer;
        private static readonly MethodInfo Set_State;
        static AiAgentCompPatch()
        {
            Set_State = AccessTools.PropertySetter(typeof(AIGoal), "State");
        }
        [HarmonyPatch(typeof(AIAgentComp), nameof(AIAgentComp.SetGoal))]
        public static class SetGoalPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(ref bool __result)
            {
                if (!Main.isConnected)
                    return true;
                if (IsFromServer)
                    return true;
                if (Main.isHost)
                    return true;
                return false;
            }
            [HarmonyPostfix]
            public static void Postfix(bool __result, AIGoal goal)
            {
                if (!Main.isConnected)
                    return;
                if (!__result)
                    return;
                if (IsFromServer)
                {
                    IsFromServer = false;
                    return;
                }
                if (!Main.isHost)
                    return;
                Set_State.Invoke(goal, new object[] { AIGoalState.Available });
                NetworkedAIGoal goalN = new NetworkedAIGoal();
                AIGoalData.TrySerialize(goal, out AIGoalData data);
                goalN.goal = MessagePackSerializer.Serialize(data);
                Printer.Warn($"Agent was {goal.Agent?.ToString() ?? "null"}");
                Printer.Warn($"Goal was {goal}");
                Printer.Error($"Job packet left with {data.Id} job id, {data.TargetId} target id, {data.AgentId} agent id");
                ListenerClient.Instance.EnqueueObject(PacketType.BroadCastNewAIGoal, goalN);
                AIGoal t = AIGoalData.Deserialize(A.S, data, false);
                Set_State.Invoke(goal, new object[] { AIGoalState.InProgress });
            }
        }
    }
}
