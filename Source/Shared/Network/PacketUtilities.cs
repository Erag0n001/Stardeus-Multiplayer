using System;
using System.Collections.Generic;
using Shared.Enums;
using Shared.Misc;
namespace Shared.Network
{
    public class PacketUtility
    {
        public static List<byte[]> CreatePacketsFromObject(PacketType target, object obj)
        {
            List<byte[]> data = Serializer.CreatePacketsFromObject(obj, target);
            return data;
        }
    }
}