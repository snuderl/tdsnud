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
 
    public class AnimatedSprite : Sprite
    {
        List<Rectangle> animations;
        //int numberOfSprites;
        //int startSprite, endSprite;
        int currentSprite;
        float sinceLastUpdate, spritesPerSecond;
        float updateInterval;

        public AnimatedSprite(Texture2D text, int width, int height, float spritesPerSecond)
            : base(text, width, height, Vector2.Zero)
        {

            sinceLastUpdate = 0;
            this.spritesPerSecond = spritesPerSecond;

            createAnimations();
            updateInterval = 1 / spritesPerSecond;
        }



        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            sinceLastUpdate += elapsed;
            if (sinceLastUpdate >=  updateInterval)
            {
                currentSprite++;
                if (currentSprite >= animations.Count)
                    currentSprite = 0;
                sinceLastUpdate -= updateInterval;
            }
        }

        public override Rectangle SourceRec { get { return animations[currentSprite]; } }

        private void createAnimations()
        {
            int numberOfSprites = text.Width * text.Height / width / height;
            animations = new List<Rectangle>(numberOfSprites);
            int line = 0;
            for (int i = 0; i < numberOfSprites; i++)
            {
                if (i * width - line * text.Width > text.Width)
                {
                    line++;
                }
                Rectangle r = new Rectangle(i * width - line * text.Width, line * height, width, height);
                animations.Add(r);
            }
        }

        public override object Clone()
        {
            AnimatedSprite ani = new AnimatedSprite(text, width, height, spritesPerSecond);
            return ani;
        }
    }

    public class Sprite : ICloneable
    {

        protected int width, height;
        public Sprite(Texture2D text, int width, int height, Vector2 startPos)
        {
            this.text = text;
            this.width = width;
            this.height = height;
            sourceRec = new Rectangle((int)startPos.X,(int)startPos.Y, width,height);
        }
        protected Texture2D text;
        private Rectangle sourceRec;
        public virtual Rectangle SourceRec { get { return sourceRec; } }
        public Texture2D Texture { get { return text; } }

        public virtual void Update(GameTime gameTime)
        { }

        public virtual object Clone()
        {
            return this;
        }
    }
}