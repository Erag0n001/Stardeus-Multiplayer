using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Game;
using Game.Components;
using Game.Data;
using HarmonyLib;
using KL.Utils;
using MessagePack;
using Multiplayer.Data;
using UnityEngine;
using static Game.UI.QuickSearch;

namespace Multiplayer.Misc
{
    public static class EntityWithCompsUtils
    {
        public static Entity Initialize(Entity entity, NetworkedObjectWithComp objN) 
        {
            entity.Definition = The.Defs.Get(entity.DefinitionId);
            Vector2 pos = A.S.Grid.ToWorldPos((int)objN.posIdx);
            entity.Position = A.S.Grid.ToWorldPos((int)objN.posIdx);
            entity.PosIdx = (int)objN.posIdx;
            entity.S = A.S;
            entity.IsActive = true;
            return SetComps(entity, objN);
        }
        public static Entity SetComps(Entity entity, NetworkedObjectWithComp objN) 
        {
            entity.Components = new IComponent[objN.comps.Count];
            for (int i = 0; i < objN.comps.Count; i++)
            {
                ComponentConfig componentConfig = entity.Definition.ComponentConfigFor(Hashes.S(objN.compDefinitions[i]), warn: false);
                IComponent comp = The.Defs.CreateComponent(componentConfig, entity);
                comp.Initialize(entity);
                if (objN.comps[i] != null) {
                    ComponentData compData = MessagePackSerializer.Deserialize<ComponentData>(objN.comps[i]);
                    comp.Data = compData;
                }
                entity.SetComponent(comp, i, objN.comps.Count);
            }
            return entity;
        }

        public static void GetComps(Entity obj, NetworkedObjectWithComp networkedObject)
        {
            foreach (IComponent comp in obj.Components)
            {
                ComponentData compData = comp.Data;
                if (compData != null)
                {
                    byte[] data = MessagePackSerializer.Serialize(compData);
                    networkedObject.comps.Add(data);
                }   
                else
                    networkedObject.comps.Add(null);
                networkedObject.compDefinitions.Add(comp.Config.Component);
            }
        }
    }
}
