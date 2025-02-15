using MessagePack;

namespace Shared.PacketData 
{
    [MessagePackObject]
    public class MouseData 
    {
        [Key(0)] public float x;
        [Key(1)] public float y;
        [Key(2)] public string? username;
    }

}