using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Core;
using Server.Data;
using Server.Misc;
using Shared.Enums;
using Shared.Misc;
using Shared.PacketData;

namespace Server.Managers
{
    public static class ValidatorManager
    {
        //First int is agent id, second is goal id
        private static Dictionary<int, Dictionary<uint, PendingGoal>> PendingGoals = new();
        public static void EnqueueNewGoal(UserClient client, NetworkedAIGoal goal) 
        {
            var pending = new PendingGoal(goal);
            if(!PendingGoals.TryGetValue(goal.agentID, out var goals))
            {
                goals = new Dictionary<uint, PendingGoal>();
                PendingGoals.Add(goal.agentID, goals);
            }
            if(goals.Count == 0) 
            {
                BroadcastManager.BroadcastToAllClient(client, 
                    Serializer.CreatePacketsFromObject(goal, PacketType.BroadCastNewAIGoal), false, true);
            }
            goals[goal.goalID] = pending;
        }

        public static void ValidateGoal(UserClient client, ValidateOrderData data) 
        {
            if(PendingGoals.TryGetValue(data.agentID, out var goals)) 
            {
                if(goals.TryGetValue(data.goalID, out var goal))
                {
                    goal.acknowlegedClients.Add(client);
                    if (goal.acknowlegedClients.Count == UserManager.ConnectedClients.Count)
                    {
                        goals.Remove(data.goalID);

                        if (goals.Count() > 0) 
                        {
                            var nextAIGoaL = goals.Values.First();
                            List<byte[]> serializedGoal = Serializer.CreatePacketsFromObject(nextAIGoaL, PacketType.BroadCastNewAIGoal);
                            BroadcastManager.BroadcastToAllClient(client, serializedGoal, false, true);
                        }
                    }
                }
            }
        }
    }

    public static class ValidatorHandler
    {
        [PacketHandler(PacketType.ValidateGoal)]
        public static void ValidateGoal(UserClient client, byte[] packet)
        {
            var data = Serializer.PacketToObject<ValidateOrderData>(packet);
            ValidatorManager.ValidateGoal(client, data);
        }
        [PacketHandler(PacketType.BroadCastNewAIGoal)]
        public static void EnqueueNewGoal(UserClient client, byte[] packet) 
        {
            if (client != MainProgram.host)
            {
                Printer.Error($"Non host tried to enqueue an AI task. This is not allowed. Disconnecting...");
                client.listener.DisconnectFlag = true;
                return;
            }
            var goal = Serializer.PacketToObject<NetworkedAIGoal>(packet);
            ValidatorManager.EnqueueNewGoal(client, goal);
        }
    }
}
