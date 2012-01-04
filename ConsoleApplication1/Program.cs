using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SnudsLib;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            float a = MathFunctions.angleBeetwenTwoVectors(new Vector2(-5,1), new Vector2(0,1));
            Console.WriteLine(a * 180 / Math.PI);
            Console.WriteLine(MathFunctions.angleFromX(new Vector2(-5,1))*180/Math.PI);
            Console.Read();
        }
    }
}
