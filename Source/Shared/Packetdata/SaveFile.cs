using MessagePack;

namespace Shared.PacketData 
{
    [MessagePackObject]
    public class SaveFile 
    {
        [Key(0)] public byte[] data;
        [Key(1)] public byte[] meta;
        [Key(2)] public string? name;
    }
}