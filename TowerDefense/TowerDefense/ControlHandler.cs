using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections;

namespace TowerDefense
{
    class ControlHandler
    {
        List<Control> controls;
        List<Enemy> enemies;

        public void checkEvents(MouseState ms)
        {
            BoundingSphere mouse = new BoundingSphere(new Vector3(ms.X, ms.Y, 1), 1);
            foreach (Control c in controls)
            {
                if(mouse.Intersects(new BoundingBox(new Vector3(c.Position, 1), new Vector3(c.Rectangle.Right, c.Rectangle.Left, 1)))){
                    c.NotifyAll();
                }
            }
            foreach (Enemy e in enemies)
            {
               
            }
        }
    }
}
