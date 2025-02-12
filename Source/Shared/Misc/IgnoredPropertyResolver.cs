namespace Shared.Misc 
{
#if CLIENT
    using MessagePack;
#endif
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class IgnoredPropertyResolver : DefaultContractResolver
    {
        public IgnoredPropertyResolver() { } // No need to store a type in the constructor

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
#if CLIENT
            property.ShouldSerialize = instance =>
            {
                if (instance == null) return true; // Avoid null reference exceptions

                Type instanceType = instance.GetType();
                if (member.GetCustomAttribute<IgnoreMemberAttribute>() != null)
                    return false;

                return true; // Serialize normally if the type is not in the dictionary
            };
#endif
            return property;
        }
    }
}