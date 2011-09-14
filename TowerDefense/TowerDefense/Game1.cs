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
using SnudsLib;

namespace TowerDefense
{

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TowerDefense : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Texture2D boxT;
        public Texture2D circleT;
        public int Lives { get; set; }
        public int money = 2000;
        public Texture2D ammoT;
        Vector2 camera;
        public int score = 0;
        public SpriteFont sf;
        public Texture2D tiles;
        public Texture2D build;
        GridTexture grid;
        public Texture2D grass;
        private Level level;
        public Level Level { get { return level; } }
        public Texture2D trap;
        public Loader loader;





        public TowerDefense()
        {

            Lives = 10;
            money = 2000;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 600;
            camera = new Vector2(0, 0);
            Components.Add(level);

            //Components.Add(gui);
            IsMouseVisible = true;

            grid = new GridTexture(this, 48);
            Components.Add(grid);


        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            boxT = Content.Load<Texture2D>(@"Textures/box");
            ammoT = Content.Load<Texture2D>(@"Textures/ammo");
            sf = Content.Load<SpriteFont>(@"Textures/font");
            circleT = Content.Load<Texture2D>(@"Textures/circle");
            tiles = Content.Load<Texture2D>(@"Textures/Tiles");
            build = Content.Load<Texture2D>(@"Textures/build");
            grass = Content.Load<Texture2D>(@"Textures/Grass_3");
            trap = Content.Load<Texture2D>(@"Textures/TradPlat");
            // TODO: use this.Content to load your game content here
            XmlDocument doc = new XmlDocument();
            doc.Load("test.xml");
            loader = new Loader(doc, Content, this);


            level = loader.levelDict[0];
            Components.Add(level);


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (level.finished == true)
            {
                level.Pause(gameTime);
            }

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState ks = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

           
            if (ks.IsKeyDown(Keys.Left))
            {
                camera.X += 200 * elapsed;
            }
            if (ks.IsKeyDown(Keys.Right))
            {
                camera.X -= 200 * elapsed;
            }
            if (ks.IsKeyDown(Keys.Up))
            {
                camera.Y += 200 * elapsed;
            }
            if (ks.IsKeyDown(Keys.Down))
            {
                camera.Y -= 200 * elapsed;
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            Level.Draw(gameTime, spriteBatch);
           
            grid.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public Vector2 OffsetPosition(Vector2 vector)
        {
            return new Vector2(vector.X + camera.X, vector.Y + camera.Y);
        }

        public Rectangle getPosition(int y, int x)
        {
            int Off = (x - 1) * 2;
            if (Off < 0)
            {
                Off = 0;
            }
            Rectangle rec = new Rectangle(4 + x * 48 + Off, 4 + y * 48 + Off, 48, 48);
            return rec;
        }
    }
}
