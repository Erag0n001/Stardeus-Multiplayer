using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;
using Game.Constants;
using Multiplayer.Misc;
using Multiplayer.Patches;
using Shared.Enums;
using Shared.Misc;
using Shared.PacketData;

namespace Multiplayer.Managers
{
    public static class MiscManager
    {
    }
    public static class MiscHandler 
    {
        [PacketHandler(PacketType.ClockSpeedChange)]
        public static void SetGameSpeed(byte[] packet) 
        {
            ClockPatch.IsFromServer = true;
            ClockSpeedData speed = Serializer.PacketToObject<ClockSpeedData>(packet);
            A.S?.Clock?.SetSpeed((ClockSpeed)speed.speed);
        }
    }
}
