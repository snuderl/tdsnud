using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SnudsLib
{
    public class MathFunctions
    {
        public static float angleBeetwenTwoVectors(Vector2 v1, Vector2 v2)
        {
           double angle = Math.Acos(Vector2.Dot(v1, v2)/v1.Length()/v2.Length());
            return (float)angle;
        }
        public static float angleFromX(Vector2 v1)
        {
            double angle = angleBeetwenTwoVectors(v1, new Vector2(1,0));
            return (float)angle;
        }

        //public static float angleBeetwenTwoVectors(Vector2 v1, Vector2 v2)
        //{
        //    float r1 = v1.X + v2.X;
        //    float r2 = v1.Y + v2.Y;
        //    float r = (float)Math.Sqrt(r1 * r1 + r2 * r2);
        //    double angle = Math.Atan(r2/r1);
        //    return (float)angle;
        //}
    }
}