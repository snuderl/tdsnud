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

namespace SnudsLib
{
    public class GridTexture : GameComponent
    {
        Game game;
        int cell;
        Camera camera;
        public GridTexture(Game game, Camera camera, int cellSize)
            : base(game)
        {
            this.game = game;
            cell = cellSize;
            this.camera = camera;
        }

        public void Draw(GameTime gameTime, SpriteBatch sb)
        {
            Texture2D texture = new Texture2D(game.GraphicsDevice, 2,2);
            texture.SetData<Color>(CreateForeground(4));
            for (int y = 0; (y-1) <= game.GraphicsDevice.Viewport.Height / cell; y++)
            {
                for (int x = 0; (x-1) <= game.GraphicsDevice.Viewport.Width / cell; x++)
                {
                    Vector2 position = new Vector2(x * cell, y * cell);
                    if (camera != null)
                    {
                        position += new Vector2(camera.Position.X % cell, camera.Position.Y % cell);
                    }
                    sb.Draw(texture,position, Color.Black);
                }
            }
        }
        private Color[] CreateForeground(int number)
        {
            Color[] foreground = new Color[number];
            for (int i = 0; i < number; i++)
            {
                foreground[i] = Color.Black;
            }
            return foreground;
        }

        ///

      
    }
}
