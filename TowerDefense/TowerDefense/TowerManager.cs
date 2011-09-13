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
        string name = "bigBadTower";
        public Sprite s;
        public int cellSize;
        public int cost;
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

        public Rectangle DestinationRectangle { get { return new Rectangle((int)position.X, (int)position.Y, cellSize, cellSize); } }
        

        public Tower(Vector2 position, int range, float shootSpeed, bool walkable, String name, int cellSize, Sprite s, int cost)
        {
            this.position = position;
            this.range = range;
            this.shootSpeed = shootSpeed;
            this.walkable = walkable;
            this.name = name;
            this.cellSize = cellSize;
            this.s = s;
            this.cost = cost;
        }


        public object Clone()
        {
            Tower t = new Tower(position, range, shootSpeed, walkable, name, cellSize, (Sprite)s.Clone(), cost);
            return t;
        }
    }

    public class TowerManager : GameComponent
    {
        TowerDefense game;
        public List<Tower> towers;

        public List<Tower> towerList;
        public TowerManager(TowerDefense game):base(game)
        {
            this.game = game;
            towers = new List<Tower>();
            towerList = new List<Tower>();
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (Tower t in towers)
            {
                t.sinceLastShot += elapsed;
                if (t.sinceLastShot >= t.shootSpeed)
                {
                    foreach (Enemy e in game.Level.EnemyManager.enemies)
                    {
                        double radius = Math.Sqrt(Math.Pow(e.position.X - t.position.X, 2) + Math.Pow(e.position.Y - t.position.Y, 2));
                        if (radius <= t.range)
                        {
                            Shoot s = new Shoot() { position = t.position, target = e, walkable = true, s = new Sprite(game.projectile, game.projectile.Width,game.projectile.Height, Vector2.Zero) };
                            game.Level.ProjectileManager.shoots.Add(s);
                            t.sinceLastShot = 0;
                            break;
                        }
                    }
                }
            }
            base.Update(gameTime);
        }

        public void Load(){
            Tower t = new Tower(new Vector2(0,0), 300, 1, false, "bigBad", 48, new Sprite(game.tower, 84, 60, new Vector2(276, 0)), 500);
            towerList.Add(t);
            t = new Tower(new Vector2(0, 0), 100, 0.1f, false, "Fastshoting", 48, new Sprite(game.tower, 84, 60, new Vector2(276, 0)), 600);
            towerList.Add(t);
            t = new Tower(new Vector2(0, 0), 600, 0.5f, false, "Kamikaze", 48, new Sprite(game.tower, 84, 60, new Vector2(276, 0)), 1000);
            towerList.Add(t);
        }

        public void Build(Vector2 position, Tower clonable)
        {
            Tower t = (Tower)clonable.Clone();
            t.position = position;
            towers.Add(t);

            game.Level.ObjectMap[(int)position.Y / 48][(int)position.X / 48] = t;
            game.Level.pathfinding = Pathfinding.createPath(game.Level.IntObjectMap, game.Level.Start, game.Level.End);
            game.Level.money -= t.cost;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Draw(GameTime gameTime, SpriteBatch sb)
        {
            foreach (Tower t in towers)
            {
                sb.Draw(t.s.Texture, t.DestinationRectangle,t.s.SourceRec, Color.White);
            }
        }


        
    }
}
