using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Game.Components;
using HarmonyLib;
using KL.Utils;
using MessagePack;
using Multiplayer.Data;
using static Game.UI.QuickSearch;

namespace Multiplayer.Misc
{
    public static class ObjectWithCompsUtils
    {
        private static object dictLock = new object();
        private static Dictionary<Type, FieldInfo[]> cachedFieldsOnObj = new Dictionary<Type, FieldInfo[]>();
        private static Dictionary<Type, MethodInfo> cachedOnSaveMethod = new Dictionary<Type, MethodInfo>();
        private static Dictionary<Type, PropertyInfo> cachedDataProperty = new Dictionary<Type, PropertyInfo>();
        private static Dictionary<Type, MethodInfo> cachedOnLoadMethod = new Dictionary<Type, MethodInfo>();
        public static T SetComps<T>(T obj, NetworkedObjectWithComp objN) 
        {
            Type mainType = typeof(T);

            if (!cachedFieldsOnObj.ContainsKey(mainType))
                CacheMainType(mainType);

            foreach (KeyValuePair<Type, byte[]> comp in objN.comps) 
            {
                try
                {
                    Type fieldType = comp.Key;
                    if (!fieldType.IsGenericType || fieldType.GetGenericTypeDefinition() != typeof(BaseComponent<>))
                    {
                        Printer.Error($"Tried deserializing a non-whitelisted type of {fieldType.Name} while creating an {typeof(T).Name}. " +
                            $"This is not allowed, you should only send childs of {typeof(BaseComponent<>).Name} for comps. skipping...");
                        continue;
                    }

                    if (!cachedOnSaveMethod.ContainsKey(fieldType))
                        CacheFieldType(fieldType);

                    FieldInfo field = null;
                    foreach (FieldInfo fieldToFind in cachedFieldsOnObj[mainType])
                    {
                        if (fieldToFind.FieldType == fieldType)
                        {
                            field = fieldToFind;
                            break;
                        }
                    }
                    if (field == null)
                    {
                        Printer.Error($"Could not find a field with type {fieldType.Name} on {mainType.Name}. Are you sure it exists? skipping...");
                        continue;
                    }

                    object component = field.GetValue(obj);
                    if (component == null)
                        field.SetValue(obj, Activator.CreateInstance(field.FieldType));

                    ComponentData data = MessagePackSerializer.Deserialize<ComponentData>(comp.Value);
                    cachedOnLoadMethod[fieldType].Invoke(field, new object[] { data });
                }
                catch (Exception ex)
                {
                    Printer.Error($"Error during deserialization of {comp.Key.Name} of the type {typeof(T)}. " +
                        $"The byte[] was of length {comp.Value.Length}.\n{ex.ToString()}");
                }
            }
            return obj;
        }

        public static void GetComps<T>(T obj, NetworkedObjectWithComp networkedObject)
        {
            Type mainType = obj.GetType();
            if(!cachedFieldsOnObj.Keys.Contains(mainType))
                CacheMainType(mainType);

            foreach (FieldInfo field in cachedFieldsOnObj[mainType]) 
            {
                Type fieldType = field.FieldType;

                if(!cachedOnSaveMethod.ContainsKey(fieldType))
                    CacheFieldType(fieldType);

                object component = field.GetValue(obj);
                if (component != null)
                {
                    cachedOnSaveMethod[fieldType].Invoke(component, null);
                    object dataValue= cachedDataProperty[fieldType].GetValue(component);
                    byte[] data = MessagePackSerializer.Serialize(dataValue);
                    networkedObject.comps.Add(fieldType, data);
                } 
            }
        }

        private static void CacheMainType(Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == typeof(BaseComponent<>)).ToArray();

            cachedFieldsOnObj.Add(type, fields);
        }

        private static void CacheFieldType(Type fieldType)
        {
            MethodInfo onSave = fieldType.GetMethod("OnSave");
            cachedOnSaveMethod.Add(fieldType, onSave);

            PropertyInfo propertyinfo = fieldType.GetProperty("Data");
            cachedDataProperty.Add(fieldType, propertyinfo);

            MethodInfo onLoad = fieldType.GetMethod("OnLoad");
            cachedOnLoadMethod.Add(fieldType, onLoad);
        }
    }
}
