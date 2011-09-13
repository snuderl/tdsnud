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
        public GridTexture(Game game, int cellSize)
            : base(game)
        {
            this.game = game;
            cell = cellSize;
        }

        public void Draw(GameTime gameTime, SpriteBatch sb)
        {
            Texture2D texture = new Texture2D(game.GraphicsDevice, 2,2);
            texture.SetData<Color>(CreateForeground(4));
            for (int y = 0; y < game.GraphicsDevice.Viewport.Height / cell; y++)
            {
                for (int x = 0; x < game.GraphicsDevice.Viewport.Width / cell; x++)
                {
                    sb.Draw(texture, new Vector2(x * cell, y * cell), Color.Black);
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


    }
}
