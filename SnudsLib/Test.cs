using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
namespace SnudsLib
{
    class Test
    {
        public static void Main()
        {
            float a =MathFunctions.angleBeetwenTwoVectors(new Vector2(0, -1), new Vector2(0, 1));
            Console.WriteLine(a);
            Console.Read();
        }
    }
}
