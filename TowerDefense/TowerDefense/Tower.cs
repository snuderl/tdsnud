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

    public class Tower : IObject, IGui, ICloneable
    {
        public Vector2 position;
        public int range;
        public float shootSpeed;
        public float sinceLastShot;
        public bool walkable;
        string name;
        public Sprite s;
        public int cellSize;
        public int cost;
        public int damage;
        public Projectile projectile;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
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

        public void Draw(GameTime gameTime, SpriteBatch sb, SpriteFont sf)
        {
            sb.DrawString(sf, this.Name, new Vector2(550, 370), Color.White);
            sb.DrawString(sf, "Atackspeed:  " + this.shootSpeed, new Vector2(550, 420), Color.White);
            sb.DrawString(sf, "Range:  " + this.range, new Vector2(550, 470), Color.White);
        }

        public void Update(GameTime gameTime)
        {
            s.Update(gameTime);
        }

        public Rectangle DestinationRectangle { get { return new Rectangle((int)position.X, (int)position.Y, cellSize, cellSize); } }


        public Tower(Vector2 position, int range, float shootSpeed, bool walkable, String name, int cellSize, Sprite s, int cost, Projectile projectile, int damage)
        {
            this.position = position;
            this.range = range;
            this.shootSpeed = shootSpeed;
            this.walkable = walkable;
            this.name = name;
            this.cellSize = cellSize;
            this.s = (Sprite)s.Clone();
            this.cost = cost;
            this.projectile = (Projectile)projectile.Clone();
            this.damage = damage;
        }


        public object Clone()
        {
            Tower t = new Tower(position, range, shootSpeed, walkable, name, cellSize, s, cost, (Projectile)projectile.Clone(), damage);
            return t;
        }
    }
}
