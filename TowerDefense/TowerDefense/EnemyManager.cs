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
    public class EnemyManager : GameComponent
    {
        TowerDefense game;
        public List<Enemy> enemies;

        public EnemyManager(TowerDefense game)
            : base(game)
        {
            this.game = game;
            enemies = new List<Enemy>();
        }

        public override void Initialize()
        {
            base.Initialize();
        }




        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (Enemy e in enemies)
            {
                Point v = e.CellCoords;
                Param p = game.Level.pathfinding.getCurrent(v);
                if (p.parent != null)
                {
                    Vector2 direction = new Vector2(p.parent.x * game.Level.CellSize + game.Level.CellSize / 2, p.parent.y * game.Level.CellSize + game.Level.CellSize / 2) - (e.position);
                    direction.Normalize();
                    e.position += direction * e.speed * elapsed;
                    e.rotation = MathFunctions.angleFromX(direction);
                    if (direction.X < 0 || direction.Y < 0)
                    {
                        e.rotation = ((float)Math.PI * 2) - e.rotation;
                    }
                }
                e.animated.Update(gameTime);

            }

            int dead = enemies.RemoveAll(e => e.health <= 0);
            int finished = enemies.RemoveAll(e => e.CellCoords == game.Level.End);
            game.Lives -= finished;
            game.score += dead * 100;
            game.money += dead * 100;
            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Enemy e in enemies)
            {
                spriteBatch.Draw(e.Text, e.destinationRectangle, e.SourceRect, Color.White, e.rotation, e.origin, SpriteEffects.None, 1);
            }
        }



        public Boolean isEnemyOnIt(Point p)
        {
            Enemy e = enemies.Find(x => (int)x.CellCoords.Y == p.Y && (int)x.CellCoords.X == p.X);
            if (e == null)
                return false;

            return true;
        }
    }
}
