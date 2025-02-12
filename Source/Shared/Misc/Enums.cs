namespace Shared.Enums 
{
    public enum Verbose { Normal, Verbose, StackTrace, FullDebug }
    public enum PacketType : byte 
    {
        ClockSpeedRequest,
        ClockSpeedChange,
        UserData,
        KeepAlive,
        MousePos,
        BroadCastNewObject,
        BroadCastNewEntity,
        BroadCastEntityDelete,
        BroadCastNewTile,
        SaveFileSend,
        RequestSaveFile,
        UserPermissions,
        BroadCastNewAIGoal,
        BroadCastNewEntityGoal
    }
}