using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Commands;
using Game.Constants;
using HarmonyLib;
using KL.Clock;
using Multiplayer.Network;
using Shared.Enums;
using Shared.PacketData;

namespace Multiplayer.Patches
{
    public static class ClockPatch
    {
        public static bool IsInitialLogin = true;
        public static bool IsFromServer = false;
        [HarmonyPatch(typeof(Clock), nameof(Clock.SetSpeed))]
        public static class SetSpeedPatch 
        {
            [HarmonyPrefix]
            public static bool Prefix() 
            {
                if (IsInitialLogin && !Main.isHost)
                {
                    ListenerClient.Instance.EnqueueObject(PacketType.ClockSpeedRequest, new KeepAliveData());
                    IsInitialLogin = false;
                    return false;
                }
                if (IsFromServer)
                    return true;
                if (!Main.isHost)
                    return false;

                return true;
            }
            [HarmonyPostfix]
            public static void Postfix(ClockSpeed newSpeed)
            {
                if (IsFromServer)
                {
                    IsFromServer = false;
                    return;
                }
                if (!Main.isHost)
                    return;
                ClockSpeedData speedData = new ClockSpeedData((NetworkedClockSpeed)newSpeed);
                ListenerClient.Instance.EnqueueObject(PacketType.ClockSpeedChange, speedData);
            }
        }
    }
}
