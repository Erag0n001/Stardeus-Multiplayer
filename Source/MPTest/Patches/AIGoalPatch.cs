using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Systems.AI;
using Game.UI;
using HarmonyLib;
using Multiplayer.Network;
using Shared.Enums;
using Shared.Misc;
using Shared.PacketData;

namespace Multiplayer.Patches
{
    public static class AIGoalPatch
    {
        [HarmonyPatch(typeof(AIGoal), nameof(AIGoal.ChangeStateTo))]
        public static class ChangeStateToPatch 
        {
            [HarmonyPostfix]
            public static void Postfix(AIGoal __instance, AIGoalState newState) 
            {
                switch (newState) 
                {
                    case AIGoalState.Completed:
                        var data = new ValidateOrderData(__instance.Agent.EntityId, __instance.Id);
                        ListenerClient.Instance.EnqueueObject(PacketType.ValidateGoal, data);
                        break;
                }
            }
        }
    }
}
