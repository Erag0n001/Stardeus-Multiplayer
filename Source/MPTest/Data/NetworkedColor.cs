using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Multiplayer.Data
{
    public class NetworkedColor
    {
        public float r;
        public float g;
        public float b;
        public float a;

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
