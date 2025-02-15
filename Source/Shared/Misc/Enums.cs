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
        BroadCastNewEntity,
        BroadCastEntityDelete,
        BroadCastNewObj,
        SaveFileSend,
        RequestSaveFile,
        UserPermissions,
        BroadCastNewAIGoal,
        BroadCastNewEntityGoal
    }
}