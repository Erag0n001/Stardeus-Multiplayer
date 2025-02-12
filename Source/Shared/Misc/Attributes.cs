using System;
using Shared.Enums;
using Shared.Network;

namespace Shared.Misc
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class PacketHandlerAttribute : Attribute 
    {
        public PacketType Header { get; }

        public PacketHandlerAttribute(PacketType header) 
        {
            Header = header;
        }
    }
}