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
using SnudsLib;
namespace TowerDefense
{
    public class Shoot
    {
        public Vector2 position;
        public Enemy target;
        public bool active = true;
        public Sprite s;
        public Rectangle destinationRectangle { get { return new Rectangle((int)position.X, (int)position.Y, 15, 15); } }
        public virtual Vector2 origin { get { return new Vector2(s.Texture.Width / 2, s.Texture.Height / 2); } }
        public float rotation;
        public Vector2 Position
        {
            get
            {
                return position;
            }
        }
        public bool Walkable
        {
            get { return walkable; }
        }

        public bool walkable;
    }
}
