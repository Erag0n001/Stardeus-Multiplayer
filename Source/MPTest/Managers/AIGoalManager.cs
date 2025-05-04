using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Game;
using Game.Data;
using Game.Systems.AI;
using Game.Systems.Creatures;
using HarmonyLib;
using MessagePack;
using Multiplayer.Data;
using Multiplayer.Misc;
using Multiplayer.Patches;
using Shared.Enums;
using Shared.Misc;
using Shared.PacketData;

namespace Multiplayer.Managers
{
    public static class AIGoalManager
    {
        static Dictionary<AIAgentComp, uint> currentAgents = new Dictionary<AIAgentComp, uint>();
        public static FixedList<AIGoal> localGoals = new FixedList<AIGoal>(25);
        public static bool CheckIfGoalHasChanged(AIAgentComp agent) 
        {
            if (!currentAgents.ContainsKey(agent))
                currentAgents.Add(agent, agent.Goal.Id);
            return currentAgents[agent] != agent.Goal.Id;
        }
    }
    public static class AIGoalHandler 
    {
        [PacketHandler(PacketType.BroadCastNewAIGoal)]
        public static void HandleNewJobFromPacket(byte[] packet)
        {
            NetworkedAIGoal goalN = Serializer.PacketToObject<NetworkedAIGoal>(packet);
            AIGoalData goalData = MessagePackSerializer.Deserialize<AIGoalData>(goalN.goal);
            AIPlanData planData = MessagePackSerializer.Deserialize<AIPlanData>(goalN.plan);
            AIGoal goal = AIGoalData.Deserialize(A.S, goalData, false);
            AIPlan plan = AIPlanData.Deserialize(planData);
            Printer.Warn($"Agent was {goal.Agent}");
            Printer.Warn($"Goal was {goal} with id {goal.Id}");
            Printer.Warn($"Plan was {plan}");
            Printer.Warn($"Target was {goal.Target} with id {goal.Target.EntityId}");
            AiAgentCompPatch.IsFromServer = true;
            Printer.Warn(goal.Agent.Goal);
            //goal.Agent.SetGoal(goal, plan, A.S.Ticks, true); Apparently this isn't needed? I am confused as well
        }
    }
}
