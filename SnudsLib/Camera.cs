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
using System.Xml;

namespace SnudsLib
{
    public class Camera : GameComponent
    {
        public Vector2 Position;
        public Point PointPosition { get { return new Point((int)Position.X, (int)Position.Y); } }
        public float Width { get; set; }
        public float Height { get; set; }

        public Camera(Game game, Vector2 startCamera, int width, int height)
            : base(game)
        {
            Position = startCamera;
            this.Width = width;
            this.Height = height;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public Rectangle offset(Rectangle rectangle)
        {
            rectangle.Offset(PointPosition);
            return rectangle;
        }
        public Vector2 offset(Vector2 position)
        {
            position += Position;
            return position;
        }


        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState ks = Keyboard.GetState();
            MouseState ms = Mouse.GetState();


            if (ks.IsKeyDown(Keys.Left))
            {
                Position.X += 200 * elapsed;
            }
            if (ks.IsKeyDown(Keys.Right))
            {
                Position.X -= 200 * elapsed;
            }
            if (ks.IsKeyDown(Keys.Up))
            {
                Position.Y += 200 * elapsed;
            }
            if (ks.IsKeyDown(Keys.Down))
            {
                Position.Y -= 200 * elapsed;
            }
            
            
            base.Update(gameTime);
        }
    }
}
