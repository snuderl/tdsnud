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
        public Texture2D grass;
        private Level level;
        public Level Level { get { return level; } }
        public Texture2D trap;
        public Loader loader;
        public SoundEffect se;
        public SoundEffect music;
        public Texture2D mountain;
        public Dictionary<String, Rectangle> sourceRectDict;
        public Texture2D health;





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
            sourceRectDict = new Dictionary<string, Rectangle>();



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
            se = Content.Load<SoundEffect>(@"Audio/fire_laser1");
            music = Content.Load<SoundEffect>(@"Audio/Theme");
            mountain = Content.Load<Texture2D>(@"Textures/004-Mountain01");
            health = Content.Load<Texture2D>(@"Textures/HealthBar2");
            // TODO: use this.Content to load your game content here
            XmlDocument doc = new XmlDocument();
            doc.Load("test.xml");
            loader = new Loader(doc, Content, this);

            sourceRectDict.Add("1", new Rectangle(0, 160, 32, 32));
            sourceRectDict.Add("2", new Rectangle(32, 160, 32, 32));
            sourceRectDict.Add("3", new Rectangle(64, 160, 32, 32));
            sourceRectDict.Add("4", new Rectangle(0, 192, 32, 32));
            sourceRectDict.Add("5", new Rectangle(0, 0, 32, 32));
            sourceRectDict.Add("6", new Rectangle(64, 192, 32, 32));
            sourceRectDict.Add("7", new Rectangle(0, 224, 32, 32));
            sourceRectDict.Add("8", new Rectangle(32, 224, 32, 32));
            sourceRectDict.Add("9", new Rectangle(64, 224, 32, 32));

            level = loader.levelDict[0];
            Components.Add(level);
            level.Start();


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

            if (level != null)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                KeyboardState ks = Keyboard.GetState();
                MouseState ms = Mouse.GetState();

                if (level.finished == true)
                {
                    level.Pause();
                    if (ks.IsKeyDown(Keys.Enter))
                    {
                        if (level.Won)
                        {
                            //Next Level
                            level.Finished();
                            if (level.Id < loader.levelDict.Count)
                            {
                                level = loader.levelDict[level.Id];
                                Components.Add(level);
                                level.Start();
                            }
                            else
                            {
                                //If there is no next level
                                level = null;
                            }
                        }
                        else if (level.Lost)
                        {
                            this.Exit();
                        }
                    }
                }



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

            if (level != null)
            {
                Level.Draw(gameTime, spriteBatch);
            }

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
