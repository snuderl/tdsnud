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

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TowerDefense : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Texture2D enemy;
        public Texture2D boxT;
        public Texture2D circleT;
        public Texture2D ammoT;
        Vector2 camera;
        public int score = 0;
        public SpriteFont sf;
        public Texture2D tiles;
        public Texture2D build;
        float sinceTowerChange = 0;
        IGui selected;
        GridTexture grid;
        public Texture2D grass;
        public Texture2D tower;
        public Texture2D projectile;
        private Level level;
        public Level Level { get { return level; } }
        private InputHandler input;
        public Tower Building { get { return level.towerManager.towerList[buildingTower]; } }
        int buildingTower;
        public Class1 inputcheck;
        public Texture2D trap;

        public enum Mode
        {
            normal, build
        }
        Mode mode = Mode.normal;




        public TowerDefense()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 600;
            camera = new Vector2(0, 0);
            level = new Level(this,48, 25, 25, new Point(0, 2), new Point(8, 9), 10);
            Components.Add(level);
            input = new InputHandler(this);
            Components.Add(input);
            Services.AddService(typeof(IInputHandler), input);
           
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
            tower = Content.Load<Texture2D>(@"Textures/GTD_SPRITES");
            enemy = Content.Load<Texture2D>(@"Textures/enemy");
            projectile = Content.Load<Texture2D>(@"Textures/thumb_Fireball");
            trap = Content.Load<Texture2D>(@"Textures/TradPlat");
            // TODO: use this.Content to load your game content here
            level.Loaded();
            buildingTower = 0;

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

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            sinceTowerChange += elapsed;
            KeyboardState ks = Keyboard.GetState();
            MouseState ms = Mouse.GetState();
            if (mode == Mode.build)
            {
                if (ButtonState.Pressed == ms.LeftButton)
                {
                    int x, y;
                    x = ms.X - (ms.X % 48);
                    y = ms.Y - (ms.Y % 48);
                    if (level.ObjectMap[y / 48][x / 48] == null && !Level.EnemyManager.isEnemyOnIt(new Point(x / 48, y / 48)) && level.money >= level.towerManager.towerList[buildingTower].cost)
                    {
                        Level.towerManager.Build(new Vector2(x, y), level.towerManager.towerList[buildingTower]);
                    }
                }
                if (sinceTowerChange > 0.1f && ks.IsKeyDown(Keys.Add))
                {
                    buildingTower++;
                    if (buildingTower == level.towerManager.towerList.Count)
                        buildingTower = 0;

                    sinceTowerChange = 0;
                }
            }
            else if (mode == Mode.normal && ButtonState.Pressed == ms.LeftButton)
            {
                Rectangle mouse = new Rectangle(ms.X - 2, ms.Y - 2, 4, 4);
                foreach (Enemy e in Level.EnemyManager.enemies)
                {
                    Rectangle rec = e.destinationRectangle;
                    if (mouse.Intersects(rec))
                    {
                        selected = e;
                    }
                }
                foreach (Tower t in Level.towerManager.towers)
                {
                    Rectangle rec = t.DestinationRectangle;
                    if (mouse.Intersects(rec))
                    {
                        selected = t;
                    }
                }
            }
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
            if (ks.IsKeyDown(Keys.B))
            {
                mode = Mode.build;
            }
            if (ks.IsKeyDown(Keys.N))
            {
                mode = Mode.normal;
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
            if (mode == Mode.build)
            {
                MouseState ms = Mouse.GetState();
                int x, y;
                x = ms.X - (ms.X % 48);
                y = ms.Y - (ms.Y % 48);
                int xCoord, yCoord;
                xCoord = x / 48;
                yCoord = y / 48;
                if (!(xCoord >= level.ObjectMap[0].Length || xCoord < 0 || yCoord < 0 || yCoord >= level.ObjectMap.Length))
                {
                    Rectangle dest = Building.DestinationRectangle;
                    dest.Offset(x, y);
                    Color c = Color.White;
                    if (level.ObjectMap[yCoord][xCoord] == null && !Level.EnemyManager.isEnemyOnIt(new Point(x / 48, y / 48)) && level.towerManager.towerList[buildingTower].cost <= level.money)
                    {
                        c = Color.LightSeaGreen;
                    }
                    else
                    {
                        c = Color.Red;
                    }

                    spriteBatch.Draw(Building.s.Texture, dest, Building.s.SourceRec, c * 0.3f);
                }
                spriteBatch.DrawString(sf, "Name: " + level.towerManager.towerList[buildingTower].Name + "\nCost: " + level.towerManager.towerList[buildingTower].cost + "\nRange: " + level.towerManager.towerList[buildingTower].range + "\nAtackSpeed: " + level.towerManager.towerList[buildingTower].shootSpeed, new Vector2(0, 400), Color.White);
            }
            else
            {
                if (selected != null)
                {
                    selected.Draw(gameTime, spriteBatch, sf);
                }
            }
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
