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
                t.Update(gameTime);
                t.sinceLastShot += elapsed;
                if (t.sinceLastShot >= t.shootSpeed)
                {
                    foreach (Enemy e in game.Level.EnemyManager.enemies)
                    {
                        double radius = Math.Sqrt(Math.Pow(e.position.X - t.position.X, 2) + Math.Pow(e.position.Y - t.position.Y, 2));
                        if (radius <= t.range)
                        {
                            Projectile s = (Projectile)t.projectile.Clone();
                            s.position = t.position;
                            s.target = e;
                            s.damage = t.damage;
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
            Projectile s = game.Level.projectileDict[1];
            Tower t = new Tower(new Vector2(0,0), 300, 1, false, "bigBad", 48, game.Level.spriteDict[2], 500, s, 10);
            towerList.Add(t);
            t = new Tower(new Vector2(0, 0), 100, 0.1f, false, "Fastshoting", 48, game.Level.spriteDict[2], 600, s, 10);
            towerList.Add(t);
            t = new Tower(new Vector2(0, 0), 600, 0.5f, false, "Kamikaze", 48, game.Level.spriteDict[2], 1000, s, 10);
            towerList.Add(t);
            s = new Projectile(200, game.Level.spriteDict[1]);
            t = new Tower(new Vector2(0, 0), 64, 5f, true, "Walking", 48, new Sprite(game.trap, 39, 39, new Vector2(0, 0)), 1000, s, 400);
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
