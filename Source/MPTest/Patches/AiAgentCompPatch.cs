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
using Shared.PacketData;

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
                __result = false;
                if (IsFromServer)
                    return true;
                if (Main.isHost)
                    return true;
                return false;
            }
            [HarmonyPostfix]
            public static void Postfix(bool __result, AIGoal goal, AIPlan plan)
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
                Set_State.Invoke(goal, new object[] { AIGoalState.PreAssigned });
                AIGoalData.TrySerialize(goal, out AIGoalData data);
                byte[] goalRaw = MessagePackSerializer.Serialize(data);
                byte[] planRaw = MessagePackSerializer.Serialize(AIPlanData.Serialize(plan));
                Printer.Warn($"Agent was {goal.Agent}");
                Printer.Warn($"Goal was {goal}");
                Printer.Warn($"Plan was {plan}");
                NetworkedAIGoal goalN = new NetworkedAIGoal(goal.Id, goal.Agent.EntityId, goalRaw, planRaw);
                ListenerClient.Instance.EnqueueObject(PacketType.BroadCastNewAIGoal, goalN);
                AIGoal t = AIGoalData.Deserialize(A.S, data, false);
                Set_State.Invoke(goal, new object[] { AIGoalState.InProgress });
            }
        }
    }
}
