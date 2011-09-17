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

namespace TowerDefense
{
    public interface IGui
    {
        void Draw(GameTime gameTime, SpriteBatch sb, SpriteFont sf);

        float radius { get; }
        Vector2 center { get; }
    }
    public interface IObject
    {
        Vector2 Position
        {
            get;
        }
        bool Walkable
        {
            get;
        }

    }

    public class End : IObject
    {

        public Vector2 Position
        {
            get { throw new NotImplementedException(); }
        }

        public bool Walkable
        {
            get { return true; }
        }
    }

}
