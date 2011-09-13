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

namespace SnudsLib
{

    public interface IInputHandler
    {
        bool isPressed(Keys k);
        bool isHolding(Keys k);
    }
    public class InputHandler : GameComponent, IInputHandler
    {
        KeyboardState keyboardState;
        KeyboardState oldKState;
        MouseState mouseState,oldMState;

        public InputHandler(Game game) : base(game) {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
        }
        public bool isPressed(Keys k)
        {
            if (keyboardState.IsKeyDown(k))
                return true;
            return false;
        }

        public bool isHolding(Keys k)
        {

            if (keyboardState.IsKeyDown(k)&&oldKState.IsKeyDown(k))
                return true;
            return false;
        }
        public override void Update(GameTime gameTime)
        {
            oldKState = keyboardState;
            oldMState = mouseState;
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            base.Update(gameTime);
        }
    }
}
