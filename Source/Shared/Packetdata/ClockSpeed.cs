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
    public class ClockSpeedData 
    {
        public ClockSpeedData(NetworkedClockSpeed speed) 
        {
            this.speed = speed;
        }
        public NetworkedClockSpeed speed;
    }
}