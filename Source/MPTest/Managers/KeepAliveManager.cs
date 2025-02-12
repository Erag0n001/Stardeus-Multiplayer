using Multiplayer.Misc;
using Shared.Enums;
using Shared.Misc;

namespace Multiplayer.Managers
{
    public static class KeepAliveHandler
    {
        [PacketHandler(PacketType.KeepAlive)]
        public static void HandlePacket(byte[] packet)
        {

        }
    }
}
