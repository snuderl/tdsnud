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

    public class Enemy : IGui, ICloneable
    {
        public Vector2 position;
        public int health;
        string name = "EvilCreep";
        public string Name { get { return name; } set { this.name = value; } }
        public Sprite animated;
        public int drawSize = 32;
        public int speed;
        public float rotation;
        public Vector2 origin { get { return new Vector2(drawSize, drawSize) / 2; } }

        public Rectangle destinationRectangle { get { return new Rectangle((int)position.X, (int)position.Y, drawSize, drawSize); } }
        public Point CellCoords
        {
            get
            {
                return new Point((int)(Position.X + origin.X) / 48, (int)(Position.Y + origin.Y) / 48);
            }
        }
        public Texture2D Text
        {
            get { return animated.Texture; }
        }
        public Rectangle SourceRect { get { return animated.SourceRec; } }
        public Vector2 Position
        {
            get
            {
                return position;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch sb, SpriteFont sf)
        {
            sb.DrawString(sf, this.Name + " " + this.CellCoords.X + ", " + this.CellCoords.Y, new Vector2(640, 400), Color.White);
            sb.DrawString(sf, "Health:  " + this.health, new Vector2(640, 450), Color.White);
        }

        public Enemy(int health, String name, Sprite ani, int drawSize, float rotation, int speed)
        {
            this.health = health;
            this.name=name;
            this.animated=(Sprite)ani.Clone();
            this.drawSize=drawSize;
            this.rotation=rotation;
            this.speed = speed;
        }

        public object Clone()
        {
            Enemy e = new Enemy(health, name, (Sprite)animated.Clone(), drawSize, 0, speed);
            return e;
        }
    }
}
