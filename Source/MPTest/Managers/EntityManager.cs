using System;
using System.Collections.Generic;
using Game;
using Game.Commands;
using Game.Data;
using Multiplayer.Data;
using Multiplayer.Misc;
using Multiplayer.Network;
using Multiplayer.Patches;
using Shared.Enums;
using Shared.Misc;
using UnityEngine;

namespace Multiplayer.Managers
{
    public static class EntityManager
    {

    }
    public static class EntityHandler
    {
        [PacketHandler(PacketType.BroadCastNewEntity)]
        public static void SpawnItemFromPacket(byte[] packet)
        {

        }
        [PacketHandler(PacketType.BroadCastEntityDelete)]
        public static void DeleteItemFromPacket(byte[] packet)
        {

        }
    }
}
