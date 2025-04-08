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
        static MethodInfo TryForceGoalMethod;
        static FieldInfo colonyGoal;
        static MethodInfo AddToConstructionGoals;
        static AIGoalHandler() 
        {
            TryForceGoalMethod = AccessTools.Method(typeof(AISys), "TryForceGoal");
            AddToConstructionGoals = AccessTools.Method(typeof(AISys), "AddToConstructionGoals");
            colonyGoal = AccessTools.Field(typeof(AISys), "colonyGoals");
        }
        [PacketHandler(PacketType.BroadCastNewAIGoal)]
        public static void HandleNewJobFromPacket(byte[] packet)
        {

            NetworkedAIGoal goalN = Serializer.PacketToObject<NetworkedAIGoal>(packet);
            AIGoalData goalData = MessagePackSerializer.Deserialize<AIGoalData>(goalN.goal);
            Printer.Warn($"Job packet arrived with {goalData.Id} job id, {goalData.TargetId} target id, {goalData.AgentId} agent id");
            AIGoal goal = AIGoalData.Deserialize(A.S, goalData, false);
            //TryForceGoalMethod.Invoke(A.S.Sys.AI, new object[] { goal, false });
            goal.Target.OnGoalLoad(goal);
            //BeingUtils.TryAssignGoal(A.S.Ticks, goal, goal.Agent, true);
            //goal.Agent.SetGoal(goal, plan, A.S.Ticks, true);
        }
        //[PacketHandler(PacketType.BroadCastNewAIGoal)]
        //public static void HandleNewEntityGoalFromPacket(byte[] packet)
        //{
        //    AiAgentCompPatch.IsFromServer = true;
        //    NetworkedAIGoal goalN = Serializer.PacketToObject<NetworkedAIGoal>(packet);
        //    AIGoalData goalData = MessagePackSerializer.Deserialize<AIGoalData>(goalN.goal);
        //    AIGoal goal = AIGoalData.Deserialize(A.S, goalData, false);
        //    Printer.Warn(goal);
        //}
    }
}
