using MessagePack;

namespace Shared.PacketData 
{
    [MessagePackObject]
    public class UserPermission 
    {
        [Key(0)] public bool isHost;
    }
}