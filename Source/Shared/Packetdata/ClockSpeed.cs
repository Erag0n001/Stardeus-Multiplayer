using MessagePack;

namespace Shared.PacketData 
{
    public enum NetworkedClockSpeed : byte
    {
        Stopped,
        Normal,
        Fast1,
        Fast2,
        Unlimited
    }
    [MessagePackObject]
    public class ClockSpeedData 
    {
        public ClockSpeedData(NetworkedClockSpeed speed) 
        {
            this.speed = speed;
        }
        [Key(0)] public NetworkedClockSpeed speed;
    }
}