using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.Magic_Wand
{
    static public class Basic_geometry
    {
        private static float do_angl = (float)57.2957;
        public static double Vec(Vec2 a, Vec2 b)
        {
            return a.x * b.y - a.y * b.x;
        }
        
        public static double Scal(Vec2 a, Vec2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static float CalculateAngle(Vec2 a, Vec2 b)
        {
            return -do_angl * ((float)Math.Atan2(b.y - a.y, b.x - a.x));
        }

        public static Vec2 GetPoint(Vec2 a, float angl, float dist)
        {
            return new Vec2(a.x + dist * (float)Math.Cos(angl), a.y + dist * (float)Math.Sin(angl));
        }

        public static float Dist(Vec2 a, Vec2 b)
        {
            return (float)Math.Sqrt((a.x + b.x) * (a.x + b.x) + (a.y + b.y) * (a.y + b.y));
        }
    }
}
