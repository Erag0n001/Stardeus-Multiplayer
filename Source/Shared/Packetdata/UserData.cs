using MessagePack;

namespace Shared.PacketData 
{
    [MessagePackObject]
    public class UserData 
    {
        [Key(0)] public string username;
    }
}