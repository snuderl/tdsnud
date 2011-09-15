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
        public TowerManager(TowerDefense game)
            : base(game)
        {
            this.game = game;
            towers = new List<Tower>();
            towerList = new List<Tower>();
        }


        #region Override

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (Tower t in towers)
            {
                t.Update(gameTime);
                t.sinceLastShot += elapsed;
                if (t.sinceLastShot >= t.shootSpeed)
                {
                    if(!(t.Target != null && t.Target.health>0 && t.range>=Radius(t, t.Target))){
                        t.Target=findClosestEnemy(t);
                    }
                    if (t.Target != null)
                    {
                        Projectile s = (Projectile)t.projectile.Clone();
                        s.position = t.position;
                        s.target = t.Target;
                        s.damage = t.damage;
                        game.Level.ProjectileManager.shoots.Add(s);
                        t.sinceLastShot = 0;
                        this.game.se .Play(0.5f,0, 0);
                    }
                }
            }
            base.Update(gameTime);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Draw(GameTime gameTime, SpriteBatch sb)
        {
            foreach (Tower t in towers)
            {
                sb.Draw(t.s.Texture, t.DestinationRectangle, t.s.SourceRec, Color.White);
            }
        }


        #endregion

        #region CustomMethods

        /// <summary>
        /// Returns enemy closest to the tower
        /// </summary>
        /// <param name="Tower"></param>
        /// <returns></returns>
        public Enemy findClosestEnemy(Tower tower)
        {
            double radius = 1000;
            Enemy selected=null;
            foreach(Enemy e in game.Level.EnemyManager.enemies)
            {
                double r = Radius(tower, e);
                if (r < radius)
                {
                    selected = e;
                    radius = r;
                }                
            }
            return selected;
        }


        /// <summary>
        /// Returns distance beetwen enemy and tower
        /// </summary>
        /// <param name="Tower"></param>
        /// <param name="Enemy"></param>
        /// <returns></returns>
        public double Radius(Tower tower, Enemy enemy)
        {
            return Math.Sqrt(Math.Pow(enemy.position.X - tower.position.X, 2) + Math.Pow(enemy.position.Y - tower.position.Y, 2));
        }


        /// <summary>
        /// Loads tower managers resources
        /// </summary>
        public void Load()
        {
            Projectile s = game.loader.projectileDict[1];
            Tower t = new Tower(new Vector2(0, 0), 300, 1, false, "bigBad", 48, game.loader.spriteDict[2], 500, s, 10);
            towerList.Add(t);
            t = new Tower(new Vector2(0, 0), 100, 0.1f, false, "Fastshoting", 48, game.loader.spriteDict[2], 600, s, 10);
            towerList.Add(t);
            t = new Tower(new Vector2(0, 0), 600, 0.5f, false, "Kamikaze", 48, game.loader.spriteDict[2], 1000, s, 10);
            towerList.Add(t);
            s = new Projectile(200, game.loader.spriteDict[1]);
            t = new Tower(new Vector2(0, 0), 64, 5f, true, "Walking", 48, new Sprite(game.trap, 39, 39, new Vector2(0, 0)), 1000, s, 400);
            towerList.Add(t);
        }

        /// <summary>
        /// Construsts selected tower at selected location
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Tower"></param>
        public void Build(Vector2 position, Tower tower)
        {
            Tower t = (Tower)tower.Clone();
            t.position = position;
            towers.Add(t);

            game.Level.ObjectMap[t.CellCoords.Y][t.CellCoords.X] = t;
            game.Level.pathfinding = Pathfinding.createPath(game.Level.IntObjectMap, new Point(0, 0), game.Level.End);
            game.money -= t.cost;
        }

        #endregion





    }
}
