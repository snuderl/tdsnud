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
    public interface IClickHandler{
        void action();
    }
    class Control
    {
        Rectangle rectangle;
        public Texture2D text;
        public Vector2 Position { get { return new Vector2(rectangle.X, rectangle.Y); } }
        public Rectangle Rectangle { get { return rectangle; } }

        List<IClickHandler> listeners;
        public void addListener(IClickHandler listener)
        {
            listeners.Add(listener);
        }
        public void removeListener(IClickHandler listener)
        {
            listeners.Remove(listener);
        }
        public void NotifyAll()
        {
            foreach (IClickHandler i in listeners)
            {
                i.action();
            }
        }
    }
}
