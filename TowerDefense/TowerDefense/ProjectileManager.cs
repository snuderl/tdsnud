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
    public class ProjectileManager : GameComponent
    {

        private TowerDefense game;
        public List<Projectile> shoots;
        public ProjectileManager(TowerDefense game)
            : base(game)
        {
            this.game = game;
            shoots = new List<Projectile>();
        }

        public override void Update(GameTime gameTime)
        {

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (Projectile s in shoots)
            {
                s.Update(gameTime);
                double radius = Math.Sqrt(Math.Pow(s.position.X - s.target.position.X, 2) + Math.Pow(s.position.Y - s.target.position.Y, 2));
                if (radius < 10)
                {
                    s.target.health -= s.damage;
                    s.active = false;
                }
                else
                {
                    Vector2 direction = (s.target.position - s.position);
                    direction.Normalize();
                    s.rotation = MathFunctions.angleFromX(direction);
                    s.position += direction * elapsed * s.speed;
                }
            }
            shoots.RemoveAll(s => s.active == false);
            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Projectile s in shoots)
            {
                spriteBatch.Draw(s.sprite.Texture, s.destinationRectangle, s.sprite.SourceRec, Color.White, s.rotation, s.origin, SpriteEffects.None, 1);
            }
        }
    }
}
