using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Game;
using Game.Data;
using Game.Systems.AI;
using HarmonyLib;
using MessagePack;
using Multiplayer.Data;
using Multiplayer.Misc;
using Multiplayer.Network;
using Shared.Enums;

namespace Multiplayer.Patches
{
    public static class EntityPatch
    {
        [HarmonyPatch(typeof(Entity), nameof(Entity.TrySetGoal))]
        public static class TrySetGoalPatch 
        {
            public static bool IsFromServer;
            private static readonly MethodInfo Set_State;
            static TrySetGoalPatch()
            {
                Set_State = AccessTools.PropertySetter(typeof(AIGoal), "State");
            }
            public static bool Prefix(bool __result)
            {
                if (IsFromServer)
                    return true;
                if (!Main.isHost)
                {
                    return false;
                }
                __result = false;
                return true;
            }
            [HarmonyPostfix]
            public static void Postfix(bool __result, AIGoal goal, Entity __instance)
            {
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
                goalN.entityID = __instance.Id;
                Printer.Warn($"Agent was {goal.Agent?.ToString() ?? "null"}");
                Printer.Error($"Job packet left with {data.Id} job id, {data.TargetId} target id, {data.AgentId} agent id");
                ListenerClient.Instance.EnqueueObject(PacketType.BroadCastNewEntityGoal, goalN);
                AIGoal t = AIGoalData.Deserialize(A.S, data, false);
                Set_State.Invoke(goal, new object[] { AIGoalState.InProgress });
            }
        }
    }
}
