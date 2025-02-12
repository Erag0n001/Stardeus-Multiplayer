using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Misc;
using Shared.Enums;
using Shared.Misc;
using Shared.PacketData;

namespace Server.Managers
{
    public static class ClockSpeedManager
    {
        private static NetworkedClockSpeed currentSpeed = NetworkedClockSpeed.Stopped;
        public static NetworkedClockSpeed CurrentSpeed => currentSpeed;
        public static void ChangeClockSpeed(UserClient client, NetworkedClockSpeed speed)
        {
            currentSpeed = speed;
            BroadcastManager.BroadcastToAllClient(client, Serializer.CreatePacketsFromObject(new ClockSpeedData(currentSpeed), PacketType.ClockSpeedChange));
        }
    }
    public static class ClockSpeedHandler 
    {
        [PacketHandler(PacketType.ClockSpeedRequest)]
        public static void RequestSpeed(UserClient client, byte[] packet)
        {
            client.listener.EnqueueObject(PacketType.ClockSpeedChange, new ClockSpeedData(ClockSpeedManager.CurrentSpeed));
        }
        [PacketHandler(PacketType.ClockSpeedChange)]
        public static void HandleClockSpeedChange(UserClient client, byte[] packet)
        {
            if (client.permission.isHost)
            {
                ClockSpeedData speed = Serializer.PacketToObject<ClockSpeedData>(packet);
                ClockSpeedManager.ChangeClockSpeed(client, speed.speed);
            }
            else
            {
                Printer.Warn($"{client.username} tried changing the speed, but he is not host. " +
                    $"This should never trigger outside of malicious packets.");
            }
        }
    }
}
