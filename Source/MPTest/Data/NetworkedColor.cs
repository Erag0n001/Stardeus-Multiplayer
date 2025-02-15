using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using UnityEngine;

namespace Multiplayer.Data
{
    [MessagePackObject]
    public class NetworkedColor
    {
        [Key(0)] public float r;
        [Key(1)] public float g;
        [Key(2)] public float b;
        [Key(3)] public float a;

        public Color ToColor() => new Color(r, g, b, a);
        public static NetworkedColor FromColor (Color c) =>
            new NetworkedColor()
            {
                r = c.r,
                g = c.g,
                b = c.b,
                a = c.a
            };
    }
}
