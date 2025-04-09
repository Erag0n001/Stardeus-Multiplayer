namespace Shared.PacketData 
{
    public class ValidateOrderData 
    {
        public ValidateOrderData(int agentId, uint goalId) 
        {
            agentID = agentId;
            goalID = goalId;
        }
        public uint goalID;
        public int agentID;
    }
}